using Ordering.API.RequestDTOs;
using Ordering.Domain.Aggregates;

namespace Ordering.API.ResponseDTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        
        public required CartDto Cart { get; set; }
        public required string CustomerUsername { get; set; }
        public Guid RestaurantId { get; set; }
        public int TotalPrice { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        
        public override string ToString()
        {
            return $"Order ID: {Id}, CustomerID: {CustomerId}, Customer: {Cart.CustomerUsername}, RestaurantID: {RestaurantId}, Total: {TotalPrice}, " +
                   $"OrderLines: {string.Join(", ", OrderLines.Select(ol => ol.ToString()))}";
        }
        
        public OrderDto(Order order)
        {
            Id = order.Id;
            CustomerId = order.CustomerId;
            CustomerUsername = order.CustomerUsername;
            RestaurantId = order.RestaurantId;
            TotalPrice = order.TotalPrice;
            OrderLines = order.OrderLines.Select(ol => new OrderLineDto
            {
                Id = ol.Id,
                Price = ol.Price,
                Quantity = ol.Quantity
            }).ToList();
        }
    }
}