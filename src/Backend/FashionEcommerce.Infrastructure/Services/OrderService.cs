using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string? userId)
    {
        // 1. Validate Items & Calculate Total
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) throw new Exception($"Product {item.ProductId} not found.");

            var unitPrice = product.BasePrice; // Simple logic: Base Price. In real app, check variants.
            var lineTotal = unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSku = product.Slug, // Using Slug as SKU for simplicity if Variants not selected
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });

            totalAmount += lineTotal;
        }

        // 2. Create Order
        var order = new Order
        {
            UserId = userId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            ShippingAddress = request.ShippingAddress,
            Note = request.Note,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            Items = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new Exception("Order not found.");

        order.Status = request.Status;
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            PaymentMethod = order.PaymentMethod.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
