using FashionEcommerce.Application.DTOs;

namespace FashionEcommerce.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string? userId);
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();
    Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);
}
