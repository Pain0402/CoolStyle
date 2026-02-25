using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FashionEcommerce.Domain.Entities;

namespace FashionEcommerce.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Define precision for Prices (Money type)
        modelBuilder.Entity<Product>()
            .Property(p => p.BasePrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductVariant>()
            .Property(p => p.PriceModifier)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        // Indexes for performance (Slug, SKU)
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<ProductVariant>()
            .HasIndex(v => v.Sku)
            .IsUnique();
    }
}
