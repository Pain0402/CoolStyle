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

    public async Task<List<ProductDto>> SearchProductsAsync(string keyword, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<ProductDto>();

        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword) || p.Category.Name.Contains(keyword))
            .Take(limit)
            .ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BasePrice = p.BasePrice,
            CategoryName = p.Category.Name,
            ThumbnailUrl = p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url ?? "",
            VariantCount = 0 // Optimization: don't load variants for search suggestions
        }).ToList();
    }

    public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId)
    {
        var reviews = await _context.ProductReviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserName = r.User.FullName,
            UserAvatar = r.User.AvatarUrl,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<ReviewDto?> AddProductReviewAsync(int productId, string userId, CreateReviewDto dto)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists) return null;

        var review = new FashionEcommerce.Domain.Entities.ProductReview
        {
            ProductId = productId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        return new ReviewDto
        {
            Id = review.Id,
            UserName = user?.FullName ?? "Unknown",
            UserAvatar = user?.AvatarUrl,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
