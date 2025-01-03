using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure;
namespace Ordering.IntegrationTests;

public class OrderIntegrationTests
{
    private static OrderingContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderingContext>()
            .UseInMemoryDatabase("testDB")
            .Options;

        return new OrderingContext(options);
    }

    [Fact]
    public void AddOrderWithOrderLine_SavesToInMemoryDatabase()
    {
        var dbContext = GetInMemoryDbContext();

        var order = new Order
        {
            CustomerId = Guid.NewGuid(),
            CustomerUsername = "john_doe",
            RestaurantId = Guid.NewGuid(),
            TotalPrice = 200
        };

        var orderLine = new OrderLine(Guid.NewGuid(), order.Id, 100, 2);
        order.OrderLines.Add(orderLine);

        dbContext.Orders.Add(order);
        dbContext.SaveChanges();

        var savedOrder = dbContext.Orders.Include(o => o.OrderLines).FirstOrDefault(o => o.Id == order.Id);
        Assert.NotNull(savedOrder);
        Assert.Equal("john_doe", savedOrder.CustomerUsername);
        Assert.Single(savedOrder.OrderLines);
        Assert.Equal(100, savedOrder.OrderLines.First().Price);
    }
    
    [Fact]
    public void GetOrderById_ReturnsCorrectOrder()
    {
        var dbContext = GetInMemoryDbContext();

        var order = new Order
        {
            CustomerId = Guid.NewGuid(),
            CustomerUsername = "john_doe",
            RestaurantId = Guid.NewGuid(),
            TotalPrice = 200
        };

        dbContext.Orders.Add(order);
        dbContext.SaveChanges();

        var retrievedOrder = dbContext.Orders.Find(order.Id);
        Assert.NotNull(retrievedOrder);
        Assert.Equal("john_doe", retrievedOrder.CustomerUsername);
    }
}