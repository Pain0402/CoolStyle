using FashionEcommerce.Application.DTOs;

namespace FashionEcommerce.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userId);
    Task UpdateProfileAsync(string userId, UpdateProfileDto model);
    
    Task<List<AddressDto>> GetAddressesAsync(string userId);
    Task<AddressDto> CreateAddressAsync(string userId, CreateAddressDto model);
    Task DeleteAddressAsync(string userId, int addressId);
    Task SetDefaultAddressAsync(string userId, int addressId);
}

public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetWishlistAsync(string userId);
    Task AddToWishlistAsync(string userId, int productId);
    Task RemoveFromWishlistAsync(string userId, int productId);
    Task<bool> IsProductInWishlistAsync(string userId, int productId);
}
