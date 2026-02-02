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

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">Register Information</param>
    /// <response code="200">Registration Successful</response>
    /// <response code="400">Validation Error</response>
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

    /// <summary>
    /// Login user.
    /// </summary>
    /// <param name="request">Login Credentials</param>
    /// <response code="200">Login Successful</response>
    /// <response code="400">Invalid Credentials</response>
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
}
