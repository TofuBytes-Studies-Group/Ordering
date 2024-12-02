using Microsoft.EntityFrameworkCore;
using Ordering.API.Repositories;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure;

namespace Ordering.APITests;

public class OrderRepositoryTest
{
    private readonly OrderRepository _orderRepository;
    private readonly OrderingContext _dbContext;
    
    public OrderRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<OrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new OrderingContext(options);
        _orderRepository = new OrderRepository(_dbContext);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = new Guid(),
            RestaurantId = new Guid(),
            CustomerUserName = "john doe",
            TotalPrice = 200,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    Id = Guid.NewGuid(),
                    Price = 100,
                    Quantity = 2
                }
            }
        };
        
        // Act
        await _orderRepository.AddAsync(order, CancellationToken.None);
        
        // Assert
        var result = await _dbContext.Orders.Include(o => o.OrderLines).FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal(order.RestaurantId, result.RestaurantId);
        Assert.Equal(order.CustomerUserName, result.CustomerUserName);
        Assert.Equal(order.TotalPrice, result.TotalPrice);
        Assert.Equal(order.OrderLines.Count, result.OrderLines.Count);
    }
}