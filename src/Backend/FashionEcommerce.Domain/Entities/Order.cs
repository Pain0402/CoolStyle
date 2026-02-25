using FashionEcommerce.Domain.Common;

namespace FashionEcommerce.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public enum PaymentMethod
{
    COD,
    VNPAY,
    MOMO
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded
}

public class Order : BaseEntity
{
    public string? UserId { get; set; } // Nullable for Guest Checkout
    public ApplicationUser? User { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Payment fields
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ProductName { get; set; } = string.Empty; // Snapshot 
    public string ProductSku { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Snapshot price at time of order
}
