using FashionEcommerce.Application.DTOs;

namespace FashionEcommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    // Task<CategoryDto?> GetCategoryBySlugAsync(string slug);
}
