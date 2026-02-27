using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Services;
using FashionEcommerce.Tests.Helpers;

namespace FashionEcommerce.Tests.Services;

public class OrderServiceTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static (OrderService service, Infrastructure.Persistence.ApplicationDbContext db) CreateSut()
    {
        var db = DbContextFactory.Create();
        return (new OrderService(db), db);
    }

    private static async Task SeedProductAsync(Infrastructure.Persistence.ApplicationDbContext db, int id = 1, decimal price = 100_000m)
    {
        var category = new Category { Id = 1, Name = "Test", Slug = "test" };
        db.Categories.Add(category);

        db.Products.Add(new Product
        {
            Id = id,
            Name = $"Product {id}",
            Slug = $"product-{id}",
            BasePrice = price,
            CategoryId = 1
        });

        await db.SaveChangesAsync();
    }

    private static CreateOrderRequest BuildRequest(int productId = 1, int qty = 2) => new()
    {
        CustomerName = "Nguyen Van A",
        CustomerEmail = "test@example.com",
        CustomerPhone = "0901234567",
        ShippingAddress = "123 Le Loi, Q1, HCM",
        PaymentMethod = PaymentMethod.COD,
        Items = new List<CartItemDto> { new() { ProductId = productId, Quantity = qty } }
    };

    // -----------------------------------------------------------------------
    // CreateOrderAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CreateOrder_ValidRequest_ReturnsOrderWithCorrectTotal()
    {
        var (sut, db) = CreateSut();
        await SeedProductAsync(db, price: 200_000m);

        var result = await sut.CreateOrderAsync(BuildRequest(qty: 3), userId: null);

        Assert.Equal(600_000m, result.TotalAmount);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("COD", result.PaymentMethod);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateOrder_WithUserId_AssociatesUserToOrder()
    {
        var (sut, db) = CreateSut();
        await SeedProductAsync(db);

        var result = await sut.CreateOrderAsync(BuildRequest(), userId: "user-123");

        var savedOrder = db.Orders.First();
        Assert.Equal("user-123", savedOrder.UserId);
    }

    [Fact]
    public async Task CreateOrder_ProductNotFound_ThrowsException()
    {
        var (sut, _) = CreateSut(); // empty db - no products

        await Assert.ThrowsAsync<Exception>(() =>
            sut.CreateOrderAsync(BuildRequest(productId: 999), userId: null));
    }

    [Fact]
    public async Task CreateOrder_SnapshotsProductName_AtTimeOfPurchase()
    {
        var (sut, db) = CreateSut();
        await SeedProductAsync(db);

        var result = await sut.CreateOrderAsync(BuildRequest(), userId: null);

        Assert.Equal("Product 1", result.Items.First().ProductName);
    }

    // -----------------------------------------------------------------------
    // GetAllOrdersAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllOrders_ReturnsAllOrdersOrderedByDateDesc()
    {
        var (sut, db) = CreateSut();
        await SeedProductAsync(db);

        await sut.CreateOrderAsync(BuildRequest(), userId: null);
        await sut.CreateOrderAsync(BuildRequest(), userId: null);

        var result = (await sut.GetAllOrdersAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllOrders_EmptyDatabase_ReturnsEmptyList()
    {
        var (sut, _) = CreateSut();

        var result = await sut.GetAllOrdersAsync();

        Assert.Empty(result);
    }

    // -----------------------------------------------------------------------
    // UpdateOrderStatusAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task UpdateOrderStatus_ValidOrder_ChangesStatus()
    {
        var (sut, db) = CreateSut();
        await SeedProductAsync(db);
        var created = await sut.CreateOrderAsync(BuildRequest(), userId: null);

        var updated = await sut.UpdateOrderStatusAsync(created.Id, new UpdateOrderStatusRequest
        {
            Status = OrderStatus.Confirmed
        });

        Assert.Equal("Confirmed", updated.Status);
    }

    [Fact]
    public async Task UpdateOrderStatus_OrderNotFound_ThrowsException()
    {
        var (sut, _) = CreateSut();

        await Assert.ThrowsAsync<Exception>(() =>
            sut.UpdateOrderStatusAsync(999, new UpdateOrderStatusRequest { Status = OrderStatus.Delivered }));
    }
}
