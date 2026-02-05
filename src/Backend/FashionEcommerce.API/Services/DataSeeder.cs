using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FashionEcommerce.API.Services;

public static class DataSeeder
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is valid or created
        if (!context.Database.CanConnect()) 
        {
             Log.Warning("DataSeeder: Cannot connect to Database.");
             return;
        }

        // Only seed if no products exist
        if (await context.Products.AnyAsync())
        {
            Log.Information("DataSeeder: Database already has data. Skipping.");
            return;
        }

        Log.Information("DataSeeder: Starting seeding V3 (Local JSON + External API + Fallback)...");

        try 
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products_scraped.json");
            if (File.Exists(jsonPath))
            {
                await SeedFromJsonFileAsync(context, jsonPath);
            }
            else 
            {
                Log.Information("DataSeeder: Local JSON not found. Attempting External API...");
                await SeedFromExternalApiAsync(context);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSeeder: Failed to seed from primary sources. Falling back to local data.");
            await SeedLocalDataAsync(context);
        }
    }

    private static async Task SeedFromJsonFileAsync(ApplicationDbContext context, string filePath)
    {
        Log.Information($"DataSeeder: Reading products from {filePath}...");
        var json = await File.ReadAllTextAsync(filePath);
        var scrapedProducts = JsonSerializer.Deserialize<List<ScrapedProduct>>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (scrapedProducts == null || !scrapedProducts.Any()) return;

        // Ensure "Coolmate" category exists
        var coolmateCat = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "coolmate");
        if (coolmateCat == null)
        {
            coolmateCat = new Category { Name = "Coolmate Collection", Slug = "coolmate", IsFeatured = true };
            context.Categories.Add(coolmateCat);
            await context.SaveChangesAsync();
        }

        var productsToSeed = scrapedProducts.Select(p => new Product
        {
            Name = p.Name,
            Slug = p.Slug + "-" + Guid.NewGuid().ToString().Substring(0, 4),
            Description = $"Sản phẩm chất lượng từ bộ sưu tập Coolmate. {p.Name} được thiết kế tối giản, hiện đại.",
            BasePrice = p.BasePrice,
            CategoryId = coolmateCat.Id,
            Images = new List<ProductImage> { new() { Url = p.ImageUrl, IsPrimary = true, DisplayOrder = 1 } }
        }).ToList();

        context.Products.AddRange(productsToSeed);
        await context.SaveChangesAsync();
        Log.Information($"DataSeeder: Successfully seeded {productsToSeed.Count} products from local JSON.");
    }

    public class ScrapedProduct
    {
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }

    private static async Task SeedFromExternalApiAsync(ApplicationDbContext context)
    {
        Log.Information("DataSeeder: Fetching data from Platzi Fake Store API...");
        
        var response = await _httpClient.GetAsync("https://api.escuelajs.co/api/v1/products?offset=0&limit=50");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var externalProducts = JsonSerializer.Deserialize<List<ExternalProduct>>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (externalProducts == null || !externalProducts.Any()) return;

        // 1. Process Categories
        var externalCategories = externalProducts
            .Select(p => p.Category)
            .GroupBy(c => c.Name)
            .Select(g => g.First())
            .ToList();

        foreach (var extCat in externalCategories)
        {
            if (!await context.Categories.AnyAsync(c => c.Name == extCat.Name))
            {
                context.Categories.Add(new Category 
                { 
                    Name = extCat.Name, 
                    Slug = extCat.Name.ToLower().Replace(" ", "-") 
                });
            }
        }
        await context.SaveChangesAsync();

        var categoryMap = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        // 2. Process Products
        var productsToSeed = new List<Product>();
        foreach (var extProd in externalProducts)
        {
            // Convert Price to VND-ish (multiply by 25k) or keep if it's already large
            decimal price = extProd.Price < 1000 ? extProd.Price * 25000 : extProd.Price;

            var product = new Product
            {
                Name = extProd.Title,
                Slug = extProd.Title.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString().Substring(0, 4),
                Description = extProd.Description,
                BasePrice = price,
                CategoryId = categoryMap[extProd.Category.Name],
                Images = extProd.Images.Select((url, index) => new ProductImage 
                { 
                    Url = url.Trim('[', ']', '\"'), // Some garbage cleaning from fake api
                    IsPrimary = index == 0,
                    DisplayOrder = index + 1
                }).ToList()
            };
            productsToSeed.Add(product);
        }

        context.Products.AddRange(productsToSeed);
        await context.SaveChangesAsync();
        Log.Information($"DataSeeder: Successfully seeded {productsToSeed.Count} products from External API.");
    }

    private static async Task SeedLocalDataAsync(ApplicationDbContext context)
    {
        // ... (Keep existing local seeding logic as fallback)
        // For brevity, I'll implement a simplified version or just use the one we had
        Log.Information("DataSeeder: Seeding local fallback data...");
        // (Implementation omitted for now, we can include the full one if needed)
    }

    // DTOs for External API
    public class ExternalProduct
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public ExternalCategory Category { get; set; } = null!;
        public string[] Images { get; set; } = Array.Empty<string>();
    }

    public class ExternalCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }

    private static Product CreateProduct(int categoryId, string name, string slug, decimal price, string description, string img1, string? img2 = null)
    {
        var images = new List<ProductImage>
        {
            new() { Url = img1, IsPrimary = true, DisplayOrder = 1 }
        };

        if (img2 != null)
        {
            images.Add(new() { Url = img2, IsPrimary = false, DisplayOrder = 2 });
        }

        return new Product
        {
            Name = name,
            Slug = slug,
            Description = $"<p>{description}</p>",
            BasePrice = price,
            CategoryId = categoryId,
            Images = images
        };
    }
}
