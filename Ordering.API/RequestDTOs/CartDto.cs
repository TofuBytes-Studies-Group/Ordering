using System.ComponentModel.DataAnnotations;

namespace Ordering.API.RequestDTOs;

public class CartDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }
    
    [Required(ErrorMessage = "Restaurant ID is required")]
    public Guid RestaurantId { get; set; }
    
    [Required(ErrorMessage = "Customer username is required")]
    public required string CustomerUsername { get; set; }
    
    [Required(ErrorMessage = "Cart items has to be provided")]
    public required List<CartItemDto> CartItems { get; set; }
    
    [Required(ErrorMessage = "Total price is required")]
    public int TotalPrice { get; set; }
}