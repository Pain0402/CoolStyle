namespace FashionEcommerce.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int VariantCount { get; set; }
}

public class ProductDetailDto : ProductDto
{
    public string Description { get; set; } = string.Empty;
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<string> Images { get; set; } = new();
}

public class ProductVariantDto
{
    public string Sku { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public decimal Price { get; set; } // Base + Modifier
    public int Stock { get; set; }
}
