namespace Ordering.API.RequestDTOs;

public class DishDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Price { get; set; }
}