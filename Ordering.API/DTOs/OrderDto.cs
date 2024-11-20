namespace Ordering.API.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string RestaurantName { get; set; }
        public int TotalPrice { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        
        public override string ToString()
        {
            return $"Order ID: {Id}, Customer: {CustomerName}, Email: {CustomerEmail}, " +
                   $"Phone: {CustomerPhoneNumber}, Address: {CustomerAddress}, Restaurant: {RestaurantName}, Total: {TotalPrice}, " +
                   $"OrderLines: {string.Join(", ", OrderLines.Select(ol => ol.ToString()))}";
        }

    }
}