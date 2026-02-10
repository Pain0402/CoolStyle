using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionEcommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WishlistItemDto>>>> Get()
    {
        var items = await _wishlistService.GetWishlistAsync(GetUserId());
        return Ok(ApiResponse<List<WishlistItemDto>>.Success(items));
    }

    [HttpGet("check/{productId}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsInWishlist(int productId)
    {
        var result = await _wishlistService.IsProductInWishlistAsync(GetUserId(), productId);
        return Ok(ApiResponse<bool>.Success(result));
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        await _wishlistService.AddToWishlistAsync(GetUserId(), productId);
        return Ok(ApiResponse<object>.Success(null, "Added to wishlist"));
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        await _wishlistService.RemoveFromWishlistAsync(GetUserId(), productId);
        return Ok(ApiResponse<object>.Success(null, "Removed from wishlist"));
    }
}
