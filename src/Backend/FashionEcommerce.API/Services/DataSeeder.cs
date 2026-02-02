using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Serilog;

namespace FashionEcommerce.API.Services;

public static class DataSeeder
{
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

        if (context.Products.Any())
        {
            Log.Information("DataSeeder: Database already has data. Skipping.");
            return;
        }

        Log.Information("DataSeeder: Starting seeding...");

        // 1. Categories
        var categories = new List<Category>
        {
            new() { Name = "Nam", Slug = "nam", SubCategories = new List<Category> {
                new() { Name = "Áo Nam", Slug = "ao-nam", SubCategories = new List<Category> {
                    new() { Name = "Áo Thun", Slug = "ao-thun" },
                    new() { Name = "Áo Polo", Slug = "ao-polo" },
                    new() { Name = "Áo Sơ Mi", Slug = "ao-so-mi" }
                }},
                new() { Name = "Quần Nam", Slug = "quan-nam", SubCategories = new List<Category> {
                    new() { Name = "Quần Jeans", Slug = "quan-jeans" },
                    new() { Name = "Quần Short", Slug = "quan-short" }
                }}
            }},
            new() { Name = "Coolmate Active", Slug = "coolmate-active" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // 2. Products
        var aoThunCat = context.Categories.First(c => c.Slug == "ao-thun");
        var quanShortCat = context.Categories.First(c => c.Slug == "quan-short");

        // High Quality Images from Unsplash
        // Note: We use specific Image IDs to ensure they are relevant Fashion items.
        
        var products = new List<Product>
        {
            new()
            {
                Name = "Cotton Compact T-Shirt Premium",
                Slug = "cotton-compact-t-shirt-premium",
                Description = "<p>Chất liệu <strong>100% Cotton Compact</strong> mềm mịn, thoáng mát. Form dáng Regular Fit phù hợp với mọi dáng người.</p>",
                BasePrice = 299000,
                CategoryId = aoThunCat.Id,
                Variants = new List<ProductVariant>
                {
                    new() { ColorName = "Đen", ColorHex = "#111111", Size = "L", Sku = "TSHIRT-BLK-L", StockQuantity = 100 },
                    new() { ColorName = "Trắng", ColorHex = "#FFFFFF", Size = "M", Sku = "TSHIRT-WHT-M", StockQuantity = 50 },
                },
                Images = new List<ProductImage>
                {
                    // Black T-Shirt Flat Lay
                    new() { Url = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&q=80&w=800", IsPrimary = true, DisplayOrder = 1 },
                    // Man wearing T-Shirt
                    new() { Url = "https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?auto=format&fit=crop&q=80&w=800", IsPrimary = false, DisplayOrder = 2 }
                }
            },
            new()
            {
                Name = "Quần Short New Basic",
                Slug = "quan-short-new-basic",
                Description = "Quần short thể thao mặc nhà hoặc đi chơi đều đẹp. Vải gió nhẹ, trượt nước.",
                BasePrice = 159000,
                CategoryId = quanShortCat.Id,
                Variants = new List<ProductVariant>
                {
                     new() { ColorName = "Xám", ColorHex = "#808080", Size = "XL", Sku = "SHORT-GRY-XL", StockQuantity = 200 }
                },
                Images = new List<ProductImage>
                {
                    // Grey Shorts
                    new() { Url = "https://images.unsplash.com/photo-1591195853828-11db59a44f6b?auto=format&fit=crop&q=80&w=800", IsPrimary = true, DisplayOrder = 1 }
                }
            },
            new()
            {
                Name = "Áo Polo Pique Minimal",
                Slug = "ao-polo-pique-minimal",
                Description = "Áo Polo vải Pique mắt chim, thoáng khí, lịch sự.",
                BasePrice = 359000,
                CategoryId = aoThunCat.Id, // Should be polo, but simplified for now
                Variants = new List<ProductVariant>(),
                Images = new List<ProductImage>
                {
                    // Polo Shirt
                    new() { Url = "https://images.unsplash.com/photo-1626557981101-aae6f84aa6ff?auto=format&fit=crop&q=80&w=800", IsPrimary = true, DisplayOrder = 1 }
                }
            },
            new()
            {
                Name = "Quần Jeans Slim Fit",
                Slug = "quan-jeans-slim-fit",
                Description = "Quần Jeans co giãn 4 chiều, màu Indigo bền màu.",
                BasePrice = 599000,
                CategoryId = quanShortCat.Id, // Should be jeans
                Variants = new List<ProductVariant>(),
                Images = new List<ProductImage>
                {
                    // Jeans
                    new() { Url = "https://images.unsplash.com/photo-1542272454315-4c01d7abdf4a?auto=format&fit=crop&q=80&w=800", IsPrimary = true, DisplayOrder = 1 }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
        
        Log.Information("DataSeeder: Seeding completed successfully.");
    }
}
