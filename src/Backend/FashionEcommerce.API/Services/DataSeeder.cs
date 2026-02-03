using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Serilog;
using Microsoft.EntityFrameworkCore;

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

        // Only seed if no products exist
        if (await context.Products.AnyAsync())
        {
            Log.Information("DataSeeder: Database already has data. Skipping.");
            return;
        }

        Log.Information("DataSeeder: Starting seeding V2 (The Big Data)...");

        // 1. Categories (Full Spectrum)
        var categories = new List<Category>
        {
            new() { Name = "Nam", Slug = "nam", SubCategories = new List<Category> {
                new() { Name = "Áo Nam", Slug = "ao-nam", SubCategories = new List<Category> {
                    new() { Name = "Áo Thun", Slug = "ao-thun" },
                    new() { Name = "Áo Polo", Slug = "ao-polo" },
                    new() { Name = "Áo Sơ Mi", Slug = "ao-so-mi" },
                    new() { Name = "Áo Khoác", Slug = "ao-khoac-nam" }
                }},
                new() { Name = "Quần Nam", Slug = "quan-nam", SubCategories = new List<Category> {
                    new() { Name = "Quần Jeans", Slug = "quan-jeans" },
                    new() { Name = "Quần Tây", Slug = "quan-tay" },
                    new() { Name = "Quần Short", Slug = "quan-short" }
                }}
            }},
            new() { Name = "Nữ", Slug = "nu", SubCategories = new List<Category> {
                 new() { Name = "Đầm & Váy", Slug = "dam-vay", SubCategories = new List<Category> {
                     new() { Name = "Đầm Liền", Slug = "dam-lien" },
                     new() { Name = "Chân Váy", Slug = "chan-vay" }
                 }},
                 new() { Name = "Áo Nữ", Slug = "ao-nu", SubCategories = new List<Category> {
                     new() { Name = "Áo Kiểu", Slug = "ao-kieu" },
                     new() { Name = "Áo Croptop", Slug = "ao-croptop" }
                 }},
                 new() { Name = "Quần Nữ", Slug = "quan-nu", SubCategories = new List<Category> {
                     new() { Name = "Quần Jeans Nữ", Slug = "quan-jeans-nu" },
                     new() { Name = "Quần Ống Rộng", Slug = "quan-ong-rong" }
                 }}
            }},
            new() { Name = "Phụ Kiện", Slug = "phu-kien", SubCategories = new List<Category> {
                new() { Name = "Nón", Slug = "non" },
                new() { Name = "Túi Xách", Slug = "tui-xach" },
                new() { Name = "Mắt Kính", Slug = "mat-kinh" }
            }},
            new() { Name = "Bộ Sưu Tập Summer 2026", Slug = "summer-2026", IsFeatured = true }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync(); // Save to generate IDs

        // Helper to get Category ID
        var catMap = await context.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id);

        // 2. Real Products (Curated List)
        var products = new List<Product>();

        // --- NAM: ÁO THUN ---
        products.Add(CreateProduct(catMap["ao-thun"], 
            "Áo Thun Basic Cotton Compact", "ao-thun-basic-cotton", 199000, 
            "Chất thun Cotton 100% định lượng 250gsm dày dặn, không xù lông.",
            "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["ao-thun"], 
            "Áo Thun Graphic Streetwear", "ao-thun-graphic", 350000, 
            "Họa tiết in lụa cao cấp, form Oversize cá tính.",
            "https://images.unsplash.com/photo-1576566588028-4147f3842f27?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1503342394128-c104d54dba01?auto=format&fit=crop&q=80&w=800"));

        // --- NAM: ÁO KHOÁC ---
        products.Add(CreateProduct(catMap["ao-khoac-nam"], 
            "Bomber Jacket Pilot", "bomber-jacket-pilot", 799000, 
            "Áo khoác Bomber vải dù chống nước nhẹ, lớp lót trần bông ấm áp.",
            "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1559551409-dadc959f76b8?auto=format&fit=crop&q=80&w=800"));

         products.Add(CreateProduct(catMap["ao-khoac-nam"], 
            "Denim Jacket Classic", "denim-jacket", 850000, 
            "Áo khoác Jeans Denim Wash bụi bặm, bền bỉ theo thời gian.",
            "https://images.unsplash.com/photo-1611312449412-6cefac5dc3e4?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1576871337622-98d48d1cf531?auto=format&fit=crop&q=80&w=800"));

        // --- NAM: QUẦN ---
        products.Add(CreateProduct(catMap["quan-jeans"], 
            "Quần Jeans Slim Fit Ripped", "quan-jeans-slim-ripped", 650000, 
            "Jeans rách gối phong cách, co giãn tốt.",
            "https://images.unsplash.com/photo-1542272454315-4c01d7abdf4a?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1475178626620-a4d074967452?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["quan-tay"], 
            "Quần Tây Âu Sidetab", "quan-tay-au-sidetab", 550000, 
            "Quần âu không cần thắt lưng với đai chỉnh Sidetab sang trọng.",
            "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?auto=format&fit=crop&q=80&w=800"));

         products.Add(CreateProduct(catMap["quan-short"], 
            "Quần Short Cargo", "quan-short-cargo", 320000, 
            "Quần short túi hộp tiện lợi, vải Kaki dày dặn.",
            "https://images.unsplash.com/photo-1591195853828-11db59a44f6b?auto=format&fit=crop&q=80&w=800"));

        // --- NỮ: ĐẦM VÁY ---
        products.Add(CreateProduct(catMap["dam-lien"], 
            "Summer Floral Dress", "summer-floral-dress", 450000, 
            "Váy hoa nhí đi biển, chất liệu Voan nhẹ nhàng bay bổng.",
            "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?auto=format&fit=crop&q=80&w=800",
            "https://images.unsplash.com/photo-1496747611176-843222e1e57c?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["dam-lien"], 
            "Elegant Evening Gown", "evening-gown", 1200000, 
            "Đầm dạ hội sang trọng, thiết kế hở lưng quyến rũ.",
            "https://images.unsplash.com/photo-1595777457583-95e059d581b8?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["chan-vay"], 
            "Chân Váy Tennis Pleated", "chan-vay-tennis", 250000, 
            "Chân váy xếp ly phong cách học đường trẻ trung.",
            "https://images.unsplash.com/photo-1582142327529-e58313e96860?auto=format&fit=crop&q=80&w=800"));

        // --- NỮ: ÁO ---
        products.Add(CreateProduct(catMap["ao-croptop"], 
            "Áo Croptop Linen", "ao-croptop-linen", 220000, 
            "Áo croptop vải đũi thoáng mát, dễ phối đồ.",
            "https://images.unsplash.com/photo-1516762689617-e1cffcef479d?auto=format&fit=crop&q=80&w=800"));
        
        products.Add(CreateProduct(catMap["ao-kieu"], 
            "Áo Sơ Mi Lụa Công Sở", "ao-so-mi-lua", 480000, 
            "Sơ mi lụa satin bóng nhẹ, thanh lịch cho môi trường công sở.",
            "https://images.unsplash.com/photo-1598532163257-ae3c6b2524b6?auto=format&fit=crop&q=80&w=800"));

        // --- PHỤ KIỆN ---
        products.Add(CreateProduct(catMap["non"], 
            "Nón Bucket Hat Retro", "bucket-hat", 150000, 
            "Nón tai bèo phong cách những năm 90.",
            "https://images.unsplash.com/photo-1575428652377-a2d80e2277fc?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["tui-xach"], 
            "Túi Tote Canvas Minimal", "tui-tote-canvas", 120000, 
            "Túi vải Canvas bảo vệ môi trường, đựng vừa Laptop 15 inch.",
            "https://images.unsplash.com/photo-1544816155-12df9643f363?auto=format&fit=crop&q=80&w=800"));

        products.Add(CreateProduct(catMap["mat-kinh"], 
            "Kính Râm Aviator", "kinh-ram-aviator", 350000, 
            "Kính phi công gọng kim loại mạ vàng.",
            "https://images.unsplash.com/photo-1572635196237-14b3f281503f?auto=format&fit=crop&q=80&w=800"));

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
        
        Log.Information($"DataSeeder: Seeding completed successfully with {products.Count} products.");
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
