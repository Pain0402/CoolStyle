using Microsoft.AspNetCore.Identity;

namespace FashionEcommerce.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<WishlistItem> Wishlist { get; set; } = new List<WishlistItem>();
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}
