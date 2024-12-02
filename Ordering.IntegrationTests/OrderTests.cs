/*using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using Ordering.Api.Controllers;
using Ordering.API.Repositories;
using Ordering.API.RequestDTOs;
using Ordering.API.ResponseDTOs;
using Ordering.API.Services;
using Ordering.API.Validators;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure;
using Xunit.Abstractions;

namespace Ordering.IntegrationTests;

public class OrderTests
{
    private readonly ITestOutputHelper _testOutput;

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

        var order = new Order
        {
            CustomerName = "john doe",
            CustomerEmail = "john@example.com",
            CustomerPhoneNumber = 12345678,
            CustomerAddress = "123 Street",
            RestaurantName = "Test Restaurant"
        };
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
        Assert.Equal(dish.Id, savedOrderLine.DishId);
        
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
        await dbContext.SaveChangesAsync();

        // Create DTO for Order
        var cartDto = new CartDto
        {
            CustomerUserName = "john_doe",
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
        var mockValidator = new Mock<CartRequestValidator>();
        var mockOrderRepository = new Mock<OrderRepository>(dbContext);
        var mockOrderService = new Mock<OrderService>(mockOrderRepository.Object, mockKafkaProducerService.Object);
        var mockOrderController = new Mock<OrderController>(dbContext, mockOrderService.Object, mockOrderRepository.Object ,mockValidator.Object);

        // Acta
        var result = await mockOrderController.Object.CreateOrder(cartDto, "John Doe", "john@example.com", 12345678, "123 Street", "Test Restaurant") as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        var createdOrder = result.Value as OrderDto;
        Assert.NotNull(createdOrder);
        Assert.Equal("John Doe", createdOrder.CustomerName);
        Assert.Single(collection: createdOrder.OrderLines ?? throw new InvalidOperationException());
        Assert.Equal("Borgor", createdOrder.OrderLines.First().DishName);
        _testOutput.WriteLine(createdOrder.OrderLines.First().DishName);
        Assert.Equal(dish.Price * 2, createdOrder.TotalPrice);
        _testOutput.WriteLine(createdOrder.ToString());
    }
}*/