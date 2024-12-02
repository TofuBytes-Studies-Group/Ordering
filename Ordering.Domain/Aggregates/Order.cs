using Ordering.Domain.Entities;

namespace Ordering.Domain.Aggregates
{
    public class Order : Interfaces.IOrder
    {
        public Order() { }

        public Order(Guid customerId)
        {
            

        }

        public Guid Id { get; init; }
        
        public Guid CustomerId { get; set; }
        
        public string CustomerUserName { get; set; }
        
        public Guid RestaurantId { get; set; }
       
        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
        public int TotalPrice { get; set; }
        public override string ToString()
        {
            return $"Order ID: {Id}, CustomerID: {CustomerId}, CustomerUserName: {CustomerUserName}, RestaurantID: {RestaurantId}, " +
                   $"TotalPrice: {TotalPrice}, OrderLine: {OrderLines}";
        }

        public Order GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}