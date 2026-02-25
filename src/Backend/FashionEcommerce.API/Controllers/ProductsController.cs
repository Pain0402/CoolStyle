using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FashionEcommerce.API.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get a paginated list of products.
    /// </summary>
    /// <param name="category">Filter by category slug (e.g., 'ao-thun')</param>
    /// <returns>List of ProductDto</returns>
    /// <response code="200">Returns list of products</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetProducts([FromQuery] string? category)
    {
        var products = await _productService.GetProductsAsync(category);
        return Ok(ApiResponse<object>.Success(products));
    }

    /// <summary>
    /// Get product details by Slug.
    /// </summary>
    /// <param name="slug">Product unique slug (e.g., 'ao-thun-cotton-compact')</param>
    /// <returns>ProductDetailDto</returns>
    /// <response code="200">Returns product details</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetProductDetail(string slug)
    {
        var product = await _productService.GetProductBySlugAsync(slug);
        if (product == null)
        {
            return NotFound(ApiResponse<object>.Error("Product not found", "NOT_FOUND"));
        }
        return Ok(ApiResponse<object>.Success(product));
    }

    /// <summary>
    /// Search products by keyword (for suggestions).
    /// </summary>
    /// <param name="q">Search keyword</param>
    /// <returns>List of ProductDto (limited)</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string q)
    {
        var products = await _productService.SearchProductsAsync(q);
        return Ok(ApiResponse<object>.Success(products));
    }

    /// <summary>
    /// Get product reviews.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>List of ReviewDto</returns>
    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetProductReviews(int id)
    {
        var reviews = await _productService.GetProductReviewsAsync(id);
        return Ok(ApiResponse<object>.Success(reviews));
    }

    /// <summary>
    /// Add a product review.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Review details (rating and comment)</param>
    /// <returns>ReviewDto</returns>
    [HttpPost("{id}/reviews")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> AddProductReview(int id, [FromBody] FashionEcommerce.Application.DTOs.CreateReviewDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var review = await _productService.AddProductReviewAsync(id, userId, dto);
        if (review == null) return NotFound(ApiResponse<object>.Error("Product not found", "NOT_FOUND"));

        return Ok(ApiResponse<object>.Success(review));
    }
}
