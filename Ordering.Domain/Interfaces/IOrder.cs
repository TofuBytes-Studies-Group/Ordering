using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Interfaces;

public interface IOrder
{
    
    
    public Guid Id { get; }
    public string CustomerName { get; }
    public string CustomerEmail { get; }
    public int CustomerPhoneNumber { get; }
    public string CustomerAddress { get; }
    public string RestaurantName { get; }
    public int TotalPrice { get; set; }
    Order? GetById(Guid id);
    IEnumerable<Order> GetAll();
}