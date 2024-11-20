using System.ComponentModel.DataAnnotations.Schema;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Aggregates
{
    public class Order : Interfaces.IOrder
    {
        private readonly List<OrderLine> _orderLines = [];
        
        public Order(){}

        public Order(string customerName, string customerEmail, int customerPhoneNumber, string customerAddress, string restaurantName)
        {
            CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
            CustomerEmail = customerEmail ?? throw new ArgumentNullException(nameof(customerEmail));
            CustomerPhoneNumber = customerPhoneNumber;
            CustomerAddress = customerAddress ?? throw new ArgumentNullException(nameof(customerAddress));
            RestaurantName = restaurantName ?? throw new ArgumentNullException(nameof(restaurantName));
        }

        public void AddOrderLine(OrderLine orderline)
        {
            ArgumentNullException.ThrowIfNull(orderline);
            _orderLines.Add(orderline);
        }

        public void RemoveOrderLine(OrderLine orderLine)
        {
            if (!_orderLines.Contains(orderLine))
                throw new InvalidOperationException("Order line does not exist.");

            _orderLines.Remove(orderLine);
        }

        public void ValidateOrder()
        {
            if (_orderLines.Count == 0)
                throw new InvalidOperationException("An order must have at least one order line.");
        }

        public void UpdateOrderLine(OrderLine orderLine, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var existingLine = _orderLines.FirstOrDefault(ol => ol.Id == orderLine.Id);
            if (existingLine == null) throw new InvalidOperationException("Order line not found.");

            existingLine.UpdateQuantity(quantity);
        }

        public Guid Id { get; }
        public string CustomerName { get; }
        public string CustomerEmail { get; }
        public int CustomerPhoneNumber { get; }
        public string CustomerAddress { get; }
        public string RestaurantName { get; }
        public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
        [NotMapped]
        public int TotalPrice => OrderLines.Sum(orderLine => orderLine.Price * orderLine.Quantity);
        


        public override string ToString()
        {
            return $"Order ID: {Id}, Customer: {CustomerName}, Email: {CustomerEmail}, " +
                   $"Phone: {CustomerPhoneNumber}, Address: {CustomerAddress}, Total: {TotalPrice}" +
                   $"Customer: {CustomerName}, Email: {CustomerEmail}, OrderLine: {OrderLines.ToString()}";
        }
        
        public int CalculateTotalPrice()
        {
            return OrderLines.Sum(ol => ol.Price * ol.Quantity);
        }
        
        public Order? GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}