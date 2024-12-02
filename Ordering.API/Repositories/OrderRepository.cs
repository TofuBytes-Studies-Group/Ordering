using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Repositories
{
    public class OrderRepository(OrderingContext dbContext) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await dbContext.Orders.AddAsync(order, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return (await dbContext.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken))!;
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Orders
                .Include(o => o.OrderLines)
                .ToListAsync(cancellationToken);
        }
    }
}