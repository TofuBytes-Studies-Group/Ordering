using Ordering.Domain.Aggregates;

namespace Ordering.Domain.Interfaces;

public interface IOrder
{
    public Guid Id { get; }
    public Guid CustomerId { get; }
    public Guid RestaurantId { get; }
    public int TotalPrice { get; }
    Order GetById(Guid id);
    IEnumerable<Order> GetAll();
}