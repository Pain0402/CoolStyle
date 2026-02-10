using FashionEcommerce.Application.DTOs;
using FashionEcommerce.Application.Interfaces;
using FashionEcommerce.Domain.Entities;
using FashionEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FashionEcommerce.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();
            
        var wishlistCount = await _context.WishlistItems.CountAsync(w => w.UserId == userId && !w.IsDeleted);

        var dto = new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Phone = user.PhoneNumber!,
            CreatedAt = user.CreatedAt,
            AvatarUrl = user.AvatarUrl,
            WishlistCount = wishlistCount,
            Addresses = addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                RecipientName = a.RecipientName,
                Phone = a.Phone,
                Street = a.Street,
                City = a.City,
                District = a.District,
                Ward = a.Ward,
                IsDefault = a.IsDefault
            }).ToList()
        };

        return dto;
    }

    public async Task UpdateProfileAsync(string userId, UpdateProfileDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        user.FullName = model.FullName;
        user.PhoneNumber = model.Phone;

        await _userManager.UpdateAsync(user);
    }

    public async Task<List<AddressDto>> GetAddressesAsync(string userId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();

        return addresses.Select(MapAddress).ToList();
    }

    public async Task<AddressDto> CreateAddressAsync(string userId, CreateAddressDto model)
    {
        // If first address, make default
        var hasDefault = await _context.Addresses.AnyAsync(a => a.UserId == userId && a.IsDefault && !a.IsDeleted);
        
        var address = new Address
        {
            UserId = userId,
            RecipientName = model.RecipientName,
            Phone = model.Phone,
            Street = model.Street,
            City = model.City,
            District = model.District,
            Ward = model.Ward,
            IsDefault = model.IsDefault || !hasDefault // First one always default
        };

        if (address.IsDefault && hasDefault)
        {
            // Reset other defaults
            var oldDefault = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault && !a.IsDeleted);
            if (oldDefault != null)
            {
                oldDefault.IsDefault = false;
                _context.Addresses.Update(oldDefault);
            }
        }

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return MapAddress(address);
    }
    
    public async Task DeleteAddressAsync(string userId, int addressId)
    {
        var address = await _context.Addresses.FindAsync(addressId);
        if (address == null || address.UserId != userId) throw new KeyNotFoundException("Address not found");
        
        address.IsDeleted = true;
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task SetDefaultAddressAsync(string userId, int addressId)
    {
        var addresses = await _context.Addresses.Where(a => a.UserId == userId && !a.IsDeleted).ToListAsync();
        var target = addresses.FirstOrDefault(a => a.Id == addressId);
        
        if (target == null) throw new KeyNotFoundException("Address not found");
        
        foreach(var addr in addresses)
        {
            addr.IsDefault = (addr.Id == addressId);
        }
        
        await _context.SaveChangesAsync();
    }

    private static AddressDto MapAddress(Address a) => new()
    {
        Id = a.Id,
        RecipientName = a.RecipientName,
        Phone = a.Phone,
        Street = a.Street,
        City = a.City,
        District = a.District,
        Ward = a.Ward,
        IsDefault = a.IsDefault
    };
}
