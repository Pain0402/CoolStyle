using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetProductsAsync(string? categorySlug = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(categorySlug))
        {
            query = query.Where(p => p.Category.Slug == categorySlug || p.Category.Parent.Slug == categorySlug);
        }

        var products = await query.ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BasePrice = p.BasePrice,
            CategoryName = p.Category.Name,
            ThumbnailUrl = p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url ?? "",
            VariantCount = p.Variants.Count
        }).ToList();
    }

    public async Task<ProductDetailDto?> GetProductBySlugAsync(string slug)
    {
        var p = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (p == null) return null;

        return new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BasePrice = p.BasePrice,
            CategoryName = p.Category.Name,
            Description = p.Description,
            ThumbnailUrl = p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url ?? "",
            Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => i.Url).ToList(),
            Variants = p.Variants.Select(v => new ProductVariantDto
            {
                Sku = v.Sku,
                ColorName = v.ColorName,
                ColorHex = v.ColorHex,
                Size = v.Size,
                Price = p.BasePrice + v.PriceModifier,
                Stock = v.StockQuantity
            }).ToList()
        };
    }
}
