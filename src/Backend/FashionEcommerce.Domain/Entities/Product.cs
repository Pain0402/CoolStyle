using FashionEcommerce.Domain.Common;

namespace FashionEcommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Markdown or HTML
    public decimal BasePrice { get; set; } // Display price
    
    // Relationships
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}
