using System.Security.Claims;
using FashionEcommerce.API.Models;
using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionEcommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;

    public OrdersController(IOrderService orderService, IConfiguration configuration)
    {
        _orderService = orderService;
        _configuration = configuration;
    }

    /// <summary>Create a new order.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CreateOrderAsync(request, userId);
            return Ok(ApiResponse<OrderResponse>.Success(result, "Order created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>Get current user's order history.</summary>
    [HttpGet("my-orders")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _orderService.GetUserOrdersAsync(userId);
        return Ok(ApiResponse<IEnumerable<OrderResponse>>.Success(result));
    }

    /// <summary>Get a specific order by ID (user can only see their own).</summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        // Admin can see any order, user can only see their own
        var result = await _orderService.GetOrderByIdAsync(id, isAdmin ? null : userId);
        if (result == null)
            return NotFound(ApiResponse<object>.Fail(null, "Order not found"));

        return Ok(ApiResponse<OrderResponse>.Success(result));
    }

    /// <summary>
    /// [Admin] Get all orders.
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrdersAsync();
        return Ok(ApiResponse<IEnumerable<OrderResponse>>.Success(result, "Fetched all orders"));
    }

    /// <summary>
    /// [Admin] Update order status.
    /// </summary>
    [HttpPut("admin/{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request);
            return Ok(ApiResponse<OrderResponse>.Success(result, "Order status updated"));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>
    /// [Mock] Get Payment URL for VNPAY.
    /// </summary>
    [HttpGet("{id}/pay")]
    public async Task<IActionResult> GetPaymentUrl(int id)
    {
        // Mocking VNPAY URL generation: Instead of actual Sandbox which requires complex hashing, just redirect to our return endpoint
        var returnUrl = $"{Request.Scheme}://{Request.Host}/api/orders/vnpay-return?vnp_TxnRef={id}&vnp_ResponseCode=00";
        // Just return our return point directly to simulate a successful payment flow
        var mockVnPayUrl = returnUrl;

        return Ok(ApiResponse<string>.Success(mockVnPayUrl, "Payment URL generated"));
    }

    /// <summary>
    /// [Mock] VNPAY IPN Webhook.
    /// </summary>
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn([FromQuery] string vnp_TxnRef, [FromQuery] string vnp_ResponseCode)
    {
        if (int.TryParse(vnp_TxnRef, out int orderId))
        {
            var request = new UpdateOrderStatusRequest 
            { 
                Status = vnp_ResponseCode == "00" ? FashionEcommerce.Domain.Entities.OrderStatus.Confirmed : FashionEcommerce.Domain.Entities.OrderStatus.Cancelled 
            };
            
            // Note: Should inject ApplicationDbContext to update PaymentStatus directly, but for now we update OrderStatus using the service. 
            // Better to trigger a payment success handler in OrderService
            await _orderService.UpdateOrderStatusAsync(orderId, request);
            
            // Get FrontendUrl from configuration
            string frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
            return Redirect($"{frontendUrl.TrimEnd('/')}/checkout/success?orderId={orderId}");
        }

        return BadRequest("Invalid transaction");
    }
}
