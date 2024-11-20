using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Api.Controllers;
using Ordering.API.DTOs;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure;
using Xunit.Abstractions;

namespace Ordering.IntegrationTests;

public class OrderTests
{
    private ITestOutputHelper _testOutput;

    public OrderTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }
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
        var orderLine = new OrderLine(Guid.NewGuid(),Guid.NewGuid(), order.Id, dish.Price, 1 );
        dbContext.OrderLines.Add(orderLine);

        dbContext.Orders.Add(order);
        dbContext.SaveChanges();
        _testOutput.WriteLine("pre-saved ORDER" + order.ToString());

        var savedOrder = dbContext.Orders.Find(order.Id);
        var savedOrderLine = dbContext.OrderLines.Find(orderLine.Id);
        var savedDish = dbContext.Dishes.Find(dish.Id);

        Assert.NotNull(savedOrder);
        _testOutput.WriteLine("SAVED ORDER" + savedOrder.ToString());
        Assert.Equal("John Doe", savedOrder.CustomerName);
        
        Assert.NotNull(savedOrderLine);
        Assert.Equal(dish.Id, savedOrderLine.Dish_Id);
        
        Assert.NotNull(savedDish);
        Assert.Equal("Pizza", savedDish.Name);
    }
    
    [Fact]
    public void CreateOrder_SavesOrderWithDishAndOrderLine()
    {
        var dbContext = GetInMemoryDbContext();

        // Seed Dish
        var dish = new Dish(Guid.NewGuid(), "Pizza", 100);
        dbContext.Dishes.Add(dish);
        dbContext.SaveChanges();

        // Create DTO for Order
        var createOrderDto = new CreateOrderDto
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            CustomerPhoneNumber = 123456789,
            CustomerAddress = "123 Street",
            RestaurantName = "Test Restaurant",
            OrderLines = [new CreateOrderLineDto { DishName = "Pizza", Quantity = 1 }],
            TotalPrice = 100 // Mocked TotalPrice, as it comes from the Cart service

        };

        // Simulate Controller
        var controller = new OrderController(dbContext);

        // Act
        var result = controller.CreateOrder(createOrderDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        var createdOrder = result.Value as OrderDto;
        Assert.NotNull(createdOrder);
        Assert.Equal("John Doe", createdOrder.CustomerName);
        Assert.Single(createdOrder.OrderLines);
        Assert.Equal("Pizza", createdOrder.OrderLines.First().DishName);
        Assert.Equal(100, createdOrder.TotalPrice); // TotalPrice should match
        _testOutput.WriteLine(createdOrder.ToString());
    }

}