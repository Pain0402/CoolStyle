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

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <param name="request">Order Information</param>
    /// <response code="200">Order Created Successfully</response>
    /// <response code="400">Validation Error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Null if Guest
            var result = await _orderService.CreateOrderAsync(request, userId);
            
            return Ok(ApiResponse<OrderResponse>.Success(result, "Order created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(null, ex.Message));
        }
    }

    /// <summary>
    /// [Admin] Get all orders.
    /// </summary>
    /// <response code="200">List of Orders</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("admin")]
    // [Authorize(Roles = "Admin")] // Uncomment when Roles are set up
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrdersAsync();
        return Ok(ApiResponse<IEnumerable<OrderResponse>>.Success(result, "Fetched all orders"));
    }

    /// <summary>
    /// [Admin] Update order status.
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="request">New Status</param>
    /// <response code="200">Order Updated Successfully</response>
    /// <response code="404">Order Not Found</response>
    [HttpPut("admin/{id}/status")]
    // [Authorize(Roles = "Admin")] // Uncomment when Roles are set up
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
        // Mocking VNPAY URL generation
        var returnUrl = $"{Request.Scheme}://{Request.Host}/api/orders/vnpay-return?vnp_TxnRef={id}&vnp_ResponseCode=00";
        var mockVnPayUrl = $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_TxnRef={id}&vnp_ReturnUrl={Uri.EscapeDataString(returnUrl)}";

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
            
            // Redirect to frontend order success page
            return Redirect($"http://localhost:5173/checkout/success?orderId={orderId}");
        }

        return BadRequest("Invalid transaction");
    }
}
