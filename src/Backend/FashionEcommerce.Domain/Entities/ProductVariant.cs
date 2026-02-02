using FashionEcommerce.Domain.Common;

namespace FashionEcommerce.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public string ColorName { get; set; } = string.Empty; // "Midnight Blue"
    public string ColorHex { get; set; } = string.Empty;  // "#123456"
    public string Size { get; set; } = string.Empty;      // "XL", "29", "30"
    
    public string Sku { get; set; } = string.Empty;       // "TSHIRT-BLUE-XL"
    public decimal PriceModifier { get; set; } = 0;       // Additional cost if any
    public int StockQuantity { get; set; } = 0;
}
