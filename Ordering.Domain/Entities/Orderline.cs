using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;

namespace Ordering.Domain.Entities;
public class OrderLine
{

    public OrderLine(){}
    public OrderLine(Guid dishId, Guid orderId, int price, int quantity)
    {
        Dish_Id = dishId;
        Order_Id = orderId;
        Price = price;
        Quantity = quantity;
    }
    public Guid Id { get; }
    public string DishName { get; }
    public Guid Dish_Id { get; }
    public Guid Order_Id { get; }
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
