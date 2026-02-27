using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FashionEcommerce.Tests.Helpers;

public static class DbContextFactory
{
    /// <summary>
    /// Creates a fresh in-memory DbContext with a unique database name per call.
    /// This ensures test isolation - each test gets a clean slate.
    /// </summary>
    public static ApplicationDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
