using Microsoft.AspNetCore.Mvc;
using FashionEcommerce.API.Models;

namespace FashionEcommerce.API.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(ApiResponse<string>.Success("FashionEcommerce API is running!", "Welcome"));
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(ApiResponse<object>.Success(new { status = "healthy", time = DateTime.UtcNow }));
    }
}
