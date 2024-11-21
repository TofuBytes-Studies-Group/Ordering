using System.ComponentModel.DataAnnotations.Schema;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Aggregates
{
    public class Order : Interfaces.IOrder
    {
        private readonly List<OrderLine> _orderLines = new();

        public Order() { }

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

        public Guid Id { get; init; }
        public string CustomerName { get; init; }
        public string CustomerEmail { get; init; }
        public int CustomerPhoneNumber { get; init; }
        public string CustomerAddress { get; init; }
        public string RestaurantName { get; init; }
        public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
        [NotMapped]
        public int TotalPrice { get; set; }

        public override string ToString()
        {
            return $"Order ID: {Id}, Customer: {CustomerName}, Email: {CustomerEmail}, " +
                   $"Phone: {CustomerPhoneNumber}, Address: {CustomerAddress}, Total: {TotalPrice}" +
                   $"Customer: {CustomerName}, Email: {CustomerEmail}, OrderLine: {OrderLines.ToString()}";
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