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
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        var profile = await _userService.GetProfileAsync(GetUserId());
        return Ok(ApiResponse<UserProfileDto>.Success(profile));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        await _userService.UpdateProfileAsync(GetUserId(), model);
        return Ok(ApiResponse<object>.Success(null, "Profile updated successfully"));
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddresses()
    {
        var addresses = await _userService.GetAddressesAsync(GetUserId());
        return Ok(ApiResponse<List<AddressDto>>.Success(addresses));
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressDto model)
    {
        var address = await _userService.CreateAddressAsync(GetUserId(), model);
        return CreatedAtAction(nameof(GetAddresses), ApiResponse<AddressDto>.Success(address));
    }

    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _userService.DeleteAddressAsync(GetUserId(), id);
        return Ok(ApiResponse<object>.Success(null, "Address deleted successfully"));
    }

    [HttpPut("addresses/{id}/default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        await _userService.SetDefaultAddressAsync(GetUserId(), id);
        return Ok(ApiResponse<object>.Success(null, "Default address updated"));
    }
}
