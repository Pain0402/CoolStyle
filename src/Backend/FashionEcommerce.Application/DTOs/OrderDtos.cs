using System.ComponentModel.DataAnnotations;
using FashionEcommerce.Domain.Entities;

namespace FashionEcommerce.Application.DTOs;

public class CreateOrderRequest
{
    [Required]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    public string ShippingAddress { get; set; } = string.Empty;

    public string? Note { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

    public List<CartItemDto> Items { get; set; } = new();
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}
