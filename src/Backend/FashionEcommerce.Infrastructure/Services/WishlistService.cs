using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly ApplicationDbContext _context;

    public WishlistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishlistItemDto>> GetWishlistAsync(string userId)
    {
        var items = await _context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Include(w => w.Product.Category) // Ensure Category is loaded
            .Where(w => w.UserId == userId && !w.IsDeleted)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

        return items.Select(w => new WishlistItemDto
        {
            Id = w.Id,
            ProductId = w.ProductId,
            AddedAt = w.CreatedAt,
            Product = new ProductDto
            {
                Id = w.Product.Id,
                Name = w.Product.Name,
                Slug = w.Product.Slug,
                BasePrice = w.Product.BasePrice,
                CategoryName = w.Product.Category?.Name ?? "",
                ThumbnailUrl = w.Product.Images.FirstOrDefault(i => i.IsPrimary)?.Url ?? ""
            }
        }).ToList();
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var exists = await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
            
        if (exists) return; // Already in wishlist

        var item = new WishlistItem
        {
            UserId = userId,
            ProductId = productId
        };
        
        _context.WishlistItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
            
        if (item == null) return;
        
        // Hard delete or soft delete? Soft delete based on BaseEntity
        item.IsDeleted = true;
        _context.WishlistItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsProductInWishlistAsync(string userId, int productId)
    {
         return await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
    }
}
