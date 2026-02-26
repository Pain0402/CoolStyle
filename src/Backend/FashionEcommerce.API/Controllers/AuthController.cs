using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FashionEcommerce.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Register a new user.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Registration successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>Login user and receive access + refresh tokens.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Login successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>
    /// Exchange a valid refresh token for a new access token + rotated refresh token.
    /// Call this when the access token expires (401 response).
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Token refreshed"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>
    /// Revoke a refresh token (logout). The token will be invalidated immediately.
    /// </summary>
    [HttpPost("revoke-token")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        try
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return Ok(ApiResponse<object>.Success(null, "Token revoked"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }
}
