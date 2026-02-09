using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FashionEcommerce.Domain.Entities;

namespace FashionEcommerce.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var entities = await _context.Categories
            .Where(c => c.ParentId == null && !c.IsDeleted) // Get root categories
            .Include(c => c.SubCategories)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return entities.Select(MapToDto).ToList();
    }

    private static CategoryDto MapToDto(Category entity)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            ParentId = entity.ParentId,
            SubCategories = entity.SubCategories
                .Where(sub => !sub.IsDeleted)
                .Select(MapToDto)
                .OrderBy(sub => sub.Name)
                .ToList()
        };
    }
}
