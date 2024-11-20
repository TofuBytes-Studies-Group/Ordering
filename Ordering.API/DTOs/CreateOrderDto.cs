namespace Ordering.API.DTOs;

public class CreateOrderDto
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public int CustomerPhoneNumber { get; set; }
    public string CustomerAddress { get; set; }
    public string RestaurantName { get; set; }
    public List<CreateOrderLineDto> OrderLines { get; set; }
}