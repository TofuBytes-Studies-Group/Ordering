using Ordering.Domain.Aggregates;

namespace Ordering.Domain.Entities;
public class OrderLine
{

    public OrderLine(){}
    public OrderLine(Guid id, Dish dish, Guid orderId, int price, int quantity)
    {
        Id = id;
        DishObject = dish;
        OrderId = orderId;
        Price = price;
        Quantity = quantity;
    }
    public Guid Id { get; }
    public Dish DishObject { get; }
    public Guid OrderId { get; }
    public int Quantity { get; private set; }
    public int Price { get; }

    
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }
        Quantity = newQuantity;
    }
}
