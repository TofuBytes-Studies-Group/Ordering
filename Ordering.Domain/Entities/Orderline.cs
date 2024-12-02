using System.Text.Json.Serialization;
using Ordering.Domain.Aggregates;

namespace Ordering.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine() { }

        public OrderLine(Guid dishId, Guid orderId, int price, int quantity)
        {
            DishId = dishId;
            OrderId = orderId;
            Price = price;
            Quantity = quantity;
        }
        
        public Guid Id { get; init; }
        [JsonIgnore]
        public Order Order { get; init; }
        public Guid DishId { get; init; }
        public Guid OrderId { get; init; }
        public int Quantity { get; init; }
        public int Price { get; init; }
        
        public override string ToString()
        {
            return $"OrderLine ID: {Id}, Quantity: {Quantity}, Price: {Price}";
        }
    }
}