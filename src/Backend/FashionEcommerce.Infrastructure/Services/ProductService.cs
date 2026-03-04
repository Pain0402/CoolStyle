using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FashionEcommerce.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public ProductService(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams queryParams)
    {
        // Build cache key from all query params
        var cacheKey = $"products:cat={queryParams.Category}:sort={queryParams.SortBy}:" +
                       $"min={queryParams.MinPrice}:max={queryParams.MaxPrice}:" +
                       $"page={queryParams.Page}:size={queryParams.PageSize}";

        // Try cache first
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<PagedResult<ProductDto>>(cached)!;
        }

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .AsNoTracking()
            .AsQueryable();

        // Filter by category
        if (!string.IsNullOrEmpty(queryParams.Category))
        {
            query = query.Where(p =>
                p.Category.Slug == queryParams.Category ||
                (p.Category.Parent != null && p.Category.Parent.Slug == queryParams.Category));
        }

        // Filter by price
        if (queryParams.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice >= queryParams.MinPrice.Value);
        if (queryParams.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= queryParams.MaxPrice.Value);

        // Sort server-side
        query = queryParams.SortBy switch
        {
            "price-asc"  => query.OrderBy(p => p.BasePrice),
            "price-desc" => query.OrderByDescending(p => p.BasePrice),
            _            => query.OrderByDescending(p => p.Id) // newest
        };

        var totalCount = await query.CountAsync();

        var page = Math.Max(1, queryParams.Page);
        var pageSize = Math.Clamp(queryParams.PageSize, 1, 100);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = products.Select(p =>
        {
            var reviewCount = p.Reviews.Count;
            var avgRating = reviewCount > 0 ? p.Reviews.Average(r => r.Rating) : 0;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                BasePrice = p.BasePrice,
                CategoryName = p.Category.Name,
                ThumbnailUrl = p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url ?? "",
                VariantCount = p.Variants.Count,
                AverageRating = Math.Round(avgRating, 1),
                ReviewCount = reviewCount
            };
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        // Store in cache
        var result = new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl });

        return result;
    }

    public async Task<ProductDetailDto?> GetProductBySlugAsync(string slug)
    {
        var p = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (p == null) return null;

        var reviewCount = p.Reviews.Count;
        var avgRating = reviewCount > 0 ? p.Reviews.Average(r => r.Rating) : 0;

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
            AverageRating = Math.Round(avgRating, 1),
            ReviewCount = reviewCount,
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
            .Where(p => p.Name.Contains(keyword) ||
                        p.Description.Contains(keyword) ||
                        p.Category.Name.Contains(keyword))
            .Take(limit)
            .ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            BasePrice = p.BasePrice,
            CategoryName = p.Category.Name,
            ThumbnailUrl = p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url ?? ""
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

        var review = new Domain.Entities.ProductReview
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
