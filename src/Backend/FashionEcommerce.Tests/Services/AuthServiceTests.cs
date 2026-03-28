using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Services;
using FashionEcommerce.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FashionEcommerce.Tests.Services;

public class AuthServiceTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var mock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        // Default: all users have "User" role — prevents NullReferenceException in BuildAuthResponseAsync
        mock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        return mock;
    }

    private static IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "SuperSecretKeyForTestingPurposesOnly123!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpiryMinutes"] = "60"
            })
            .Build();

    private static ApplicationUser MakeUser(string email = "user@test.com") => new()
    {
        Id = Guid.NewGuid().ToString(),
        Email = email,
        UserName = email,
        FullName = "Test User"
    };

    // -----------------------------------------------------------------------
    // RegisterAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Register_NewEmail_ReturnsTokenAndRefreshToken()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<ApplicationUser, string>((u, _) => u.Id = user.Id);

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        var result = await sut.RegisterAsync(new RegisterDto
        {
            FullName = "Test User",
            Email = user.Email!,
            Password = "Password123!"
        });

        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task Register_ExistingEmail_ThrowsException()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await Assert.ThrowsAsync<Exception>(() =>
            sut.RegisterAsync(new RegisterDto { Email = user.Email!, FullName = "X", Password = "P@ss1" }));
    }

    [Fact]
    public async Task Register_AdminEmail_ReturnsAdminRole()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var adminUser = MakeUser("admin@coolstyle.com");

        userManager.Setup(m => m.FindByEmailAsync(adminUser.Email!)).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<ApplicationUser, string>((u, _) => u.Id = adminUser.Id);

        // Admin user returns Admin role
        userManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Admin" });

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        var result = await sut.RegisterAsync(new RegisterDto
        {
            FullName = "Admin",
            Email = adminUser.Email!,
            Password = "Admin@123"
        });

        Assert.Equal("Admin", result.Role);
    }

    // -----------------------------------------------------------------------
    // LoginAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        var result = await sut.LoginAsync(new LoginDto { Email = user.Email!, Password = "Password123!" });

        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsException()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await Assert.ThrowsAsync<Exception>(() =>
            sut.LoginAsync(new LoginDto { Email = user.Email!, Password = "WrongPass" }));
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsException()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();

        userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await Assert.ThrowsAsync<Exception>(() =>
            sut.LoginAsync(new LoginDto { Email = "ghost@test.com", Password = "any" }));
    }

    // -----------------------------------------------------------------------
    // RefreshTokenAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        // Seed a valid refresh token directly in DB
        db.Users.Add(user);
        var existingToken = new RefreshToken
        {
            Token = "valid-refresh-token",
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        db.RefreshTokens.Add(existingToken);
        await db.SaveChangesAsync();

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        var result = await sut.RefreshTokenAsync("valid-refresh-token");

        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
        // Old token should be revoked (rotation)
        Assert.True(existingToken.IsRevoked);
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_ThrowsException()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        db.Users.Add(user);
        db.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(-1) // already expired
        });
        await db.SaveChangesAsync();

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await Assert.ThrowsAsync<Exception>(() => sut.RefreshTokenAsync("expired-token"));
    }

    [Fact]
    public async Task RefreshToken_RevokedToken_ThrowsException()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        db.Users.Add(user);
        db.RefreshTokens.Add(new RefreshToken
        {
            Token = "revoked-token",
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = true
        });
        await db.SaveChangesAsync();

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await Assert.ThrowsAsync<Exception>(() => sut.RefreshTokenAsync("revoked-token"));
    }

    // -----------------------------------------------------------------------
    // RevokeTokenAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RevokeToken_ValidToken_SetsIsRevokedTrue()
    {
        var userManager = CreateUserManagerMock();
        var db = DbContextFactory.Create();
        var user = MakeUser();

        db.Users.Add(user);
        var token = new RefreshToken
        {
            Token = "active-token",
            UserId = user.Id,
            User = user,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var sut = new AuthService(userManager.Object, CreateConfig(), db);

        await sut.RevokeTokenAsync("active-token");

        Assert.True(token.IsRevoked);
    }
}
