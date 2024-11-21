using Ordering.Domain.Aggregates;

namespace Ordering.Domain.Interfaces;

public interface IOrderService
{
    Task CreateOrderAsync(Order order, CancellationToken cancellationToken);
}