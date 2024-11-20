namespace Ordering.API.DTOs;

public class CreateOrderLineDto
{
    public string DishName { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
}
