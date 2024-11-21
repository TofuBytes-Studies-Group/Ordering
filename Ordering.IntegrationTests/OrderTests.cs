using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using Ordering.Api.Controllers;
using Ordering.API.DTOs;
using Ordering.API.Repositories;
using Ordering.API.RequestDTOs;
using Ordering.API.Services;
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

        var order = new Order("John Doe", "john@example.com", 123456789, "123 Street", "Test Restaurant");
        var orderLine = new OrderLine(dish.Id, order.Id, dish.Price, 2, dish.Name);       
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
        _testOutput.WriteLine("SAVED ORDERLINE" + savedOrderLine.ToString());
        Assert.Equal(dish.Id, savedOrderLine.Dish_Id);
        
        Assert.NotNull(savedDish);
        Assert.Equal("Pizza", savedDish.Name);
    }
    
    [Fact]
    public async Task CreateOrder_SavesOrderWithDishAndOrderLine()
    {
        var dbContext = GetInMemoryDbContext();

        // Seed Dish
        var dish = new Dish(Guid.NewGuid(), "Borgor", 100);
        dbContext.Dishes.Add(dish);
        dbContext.SaveChanges();

        // Create DTO for Order
        var cartDto = new CartDto
        {
            Username = "john_doe",
            CartItems = new List<CartItemDto>
            {
                new CartItemDto
                {
                    Dish = new DishDto { Id = dish.Id, Name = dish.Name, Price = dish.Price },
                    Quantity = 2,
                    SumPrice = dish.Price * 2
                }
            },
            TotalPrice = dish.Price * 2
        };

        var mockKafkaProducerService = new Mock<IKafkaProducerService>();
        mockKafkaProducerService
            .Setup(service => service.ProduceOrderAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Simulate Controller
        var orderService = new OrderService(new OrderRepository(dbContext), mockKafkaProducerService.Object);
        var controller = new OrderController(dbContext, orderService);

        // Act
        var result = await controller.CreateOrder(cartDto, "John Doe", "john@example.com", 123456789, "123 Street", "Test Restaurant") as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        var createdOrder = result.Value as OrderDto;
        Assert.NotNull(createdOrder);
        Assert.Equal("John Doe", createdOrder.CustomerName);
        Assert.Single(createdOrder.OrderLines);
        Assert.Equal("Borgor", createdOrder.OrderLines.First().DishName);
        _testOutput.WriteLine(createdOrder.OrderLines.First().DishName);
        Assert.Equal(dish.Price * 2, createdOrder.TotalPrice);
        _testOutput.WriteLine(createdOrder.ToString());
    }
}