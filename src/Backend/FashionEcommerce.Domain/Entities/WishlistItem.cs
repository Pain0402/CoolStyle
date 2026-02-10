using FashionEcommerce.Domain.Common;

namespace FashionEcommerce.Domain.Entities;

public class WishlistItem : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
