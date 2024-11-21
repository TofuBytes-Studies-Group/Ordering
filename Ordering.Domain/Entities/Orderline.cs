namespace Ordering.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine() { }

        public OrderLine(Guid dishId, Guid orderId, int price, int quantity, string dishName)
        {
            Dish_Id = dishId;
            Order_Id = orderId;
            Price = price;
            Quantity = quantity;
            DishName = dishName;
        }

        public Guid Id { get; init; }
        public string DishName { get; init; }
        public Guid Dish_Id { get; init; }
        public Guid Order_Id { get; init; }
        public int Quantity { get; init; }
        public int Price { get; init; }
        
        public override string ToString()
        {
            return $"OrderLine ID: {Id}, Dish: {DishName}, Quantity: {Quantity}, Price: {Price}";
        }
    }
}