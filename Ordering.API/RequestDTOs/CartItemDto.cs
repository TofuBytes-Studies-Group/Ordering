namespace Ordering.API.RequestDTOs;

public class CartItemDto
{
    public DishDto Dish { get; set; }
    public int Quantity { get; set; }
    public int SumPrice { get; set; }
}