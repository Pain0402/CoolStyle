using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Application.DTOs;

public class AddressDto
{
    public int Id { get; set; }
    
    [Required]
    public string RecipientName { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public string Street { get; set; } = string.Empty;
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    [Required]
    public string District { get; set; } = string.Empty;
    
    public string Ward { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; }
}

public class CreateAddressDto
{
    [Required]
    public string RecipientName { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public string Street { get; set; } = string.Empty;
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    [Required]
    public string District { get; set; } = string.Empty;
    
    public string Ward { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; }
}
