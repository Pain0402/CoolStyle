using FashionEcommerce.API.Models;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Produces("application/json")]
// [Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get revenue statistics for admin dashboard.
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueStats()
    {
        var totalOrders = await _context.Orders.CountAsync();
        
        // Sum of all non-cancelled orders
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != FashionEcommerce.Domain.Entities.OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var recentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new {
                o.Id,
                o.CustomerName,
                o.TotalAmount,
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                o.CreatedAt
            })
            .ToListAsync();

        // Revenue grouped by date for the last 7 days (mock representation)
        var last7Days = Enumerable.Range(0, 7).Select(i => DateTime.UtcNow.Date.AddDays(-i)).Reverse().ToList();
        var chartData = new List<object>();

        var ordersIn7Days = await _context.Orders
            .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7) && o.Status != FashionEcommerce.Domain.Entities.OrderStatus.Cancelled)
            .ToListAsync();

        foreach (var date in last7Days)
        {
            var dailyRevenue = ordersIn7Days.Where(o => o.CreatedAt.Date == date).Sum(o => o.TotalAmount);
            chartData.Add(new { Date = date.ToString("yyyy-MM-dd"), Revenue = dailyRevenue });
        }

        return Ok(ApiResponse<object>.Success(new
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            ChartData = chartData,
            RecentOrders = recentOrders
        }));
    }
}
