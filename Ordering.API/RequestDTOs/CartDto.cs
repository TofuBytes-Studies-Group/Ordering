namespace Ordering.API.RequestDTOs;

public class CartDto
{
    public string Username { get; set; }
    public List<CartItemDto> CartItems { get; set; }
    public int TotalPrice { get; set; }
}