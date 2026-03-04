using FashionEcommerce.Application.DTOs;

namespace FashionEcommerce.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams queryParams);
    Task<ProductDetailDto?> GetProductBySlugAsync(string slug);
    Task<List<ProductDto>> SearchProductsAsync(string keyword, int limit = 10);
    Task<List<ReviewDto>> GetProductReviewsAsync(int productId);
    Task<ReviewDto?> AddProductReviewAsync(int productId, string userId, CreateReviewDto dto);
}
