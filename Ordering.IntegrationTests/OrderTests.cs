using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure;

namespace Ordering.IntegrationTests;

public class OrderTests
{
    private OrderingContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderingContext>()
            .UseInMemoryDatabase("testDB")
            .Options;

        return new OrderingContext(options);
    }

    [Fact]
    public void AddOrderWithDish_SavesToInMemoryDatabase()
    {

        var dbContext = GetInMemoryDbContext();
        
        var dish = new Dish(Guid.NewGuid(), "Pizza", 100);
        dbContext.Dishes.Add(dish);

        var order = new Order(Guid.NewGuid(), "John Doe", "john@example.com", 123456789, "123 Street", "Test Restaurant");
        var orderLine = new OrderLine(Guid.NewGuid(),dish, order.Id, dish.Price, 1 );
        dbContext.OrderLines.Add(orderLine);

        dbContext.Orders.Add(order);
        dbContext.SaveChanges();

        var savedOrder = dbContext.Orders.Find(order.Id);
        var savedOrderLine = dbContext.OrderLines.Find(orderLine.Id);
        var savedDish = dbContext.Dishes.Find(dish.Id);

        Assert.NotNull(savedOrder);
        Assert.Equal("John Doe", savedOrder.CustomerName);
        Assert.NotNull(savedOrderLine);
        Assert.Equal(dish.Id, savedOrderLine.DishObject.Id);
        Assert.NotNull(savedDish);
        Assert.Equal("Pizza", savedDish.Name);
    }
}