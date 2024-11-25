using System.Linq.Expressions;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ordering.Api.Controllers;
using Ordering.API.DTOs;
using Ordering.API.RequestDTOs;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure;
using Sprache;

namespace Ordering.APITests;

public class OrderControllerTest
{
    public OrderControllerTest()
    {
        Env.Load();
    }

    [Fact]
    public async Task CreateOrder_ValidCartDto_ReturnsCreatedAtActionResult()
    {
        var dbContextMock = new Mock<OrderingContext>(new DbContextOptions<OrderingContext>());
        var orderServiceMock = new Mock<IOrderService>();
        var controller = new OrderController(dbContextMock.Object, orderServiceMock.Object);

        var cartDto = new CartDto
        {
            Username = "john_doe",
            CartItems = new List<CartItemDto>
            {
                new CartItemDto
                {
                    Dish = new DishDto { Id = Guid.NewGuid(), Name = "Pizza", Price = 100 },
                    Quantity = 2,
                    SumPrice = 200
                }
            },
            TotalPrice = 200
        };

        var customerName = Environment.GetEnvironmentVariable("CUSTOMER_NAME");
        var customerEmail = Environment.GetEnvironmentVariable("CUSTOMER_EMAIL");
        var customerPhoneNumber = Convert.ToInt32(Environment.GetEnvironmentVariable("CUSTOMER_PHONE_NUMBER"));
        var customerAddress = Environment.GetEnvironmentVariable("CUSTOMER_ADDRESS");
        var restaurantName = Environment.GetEnvironmentVariable("RESTAURANT_NAME");

        var result = await controller.CreateOrder(cartDto, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);

        Assert.IsType<CreatedAtActionResult>(result);
    }
    
    [Fact]
    public async Task CreateOrder_NullCartDto_ReturnsBadObjectRequest()
    {
        var dbContextMock = new Mock<OrderingContext>(new DbContextOptions<OrderingContext>());
        var orderServiceMock = new Mock<IOrderService>();
        var controller = new OrderController(dbContextMock.Object, orderServiceMock.Object);

        var customerName = Environment.GetEnvironmentVariable("CUSTOMER_NAME");
        var customerEmail = Environment.GetEnvironmentVariable("CUSTOMER_EMAIL");
        var customerPhoneNumber = Convert.ToInt32(Environment.GetEnvironmentVariable("CUSTOMER_PHONE_NUMBER"));
        var customerAddress = Environment.GetEnvironmentVariable("CUSTOMER_ADDRESS");
        var restaurantName = Environment.GetEnvironmentVariable("RESTAURANT_NAME");

        var result = await controller.CreateOrder(null, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
[Fact]
public void GetOrder_ExistingOrderId_ReturnsOrderDto()
{
    // fuck it idk how else to do it rn.. might aswell have hardcoded it all since this was the only fix i could come up with atm but here goes
     Environment.SetEnvironmentVariable("CUSTOMER_NAME", "Pete Zar-riah Firenze");
     Environment.SetEnvironmentVariable("CUSTOMER_EMAIL", "gabbagool@gogglemail.com");
     Environment.SetEnvironmentVariable("CUSTOMER_PHONE_NUMBER", "12345678");
     Environment.SetEnvironmentVariable("CUSTOMER_ADDRESS", "123 Fake Street");
     Environment.SetEnvironmentVariable("RESTAURANT_NAME", "genereic pizza place");

    var dbContextMock = new Mock<OrderingContext>(new DbContextOptions<OrderingContext>());
    var orderServiceMock = new Mock<IOrderService>();
    var controller = new OrderController(dbContextMock.Object, orderServiceMock.Object);
    var ordersDbSetMock = new Mock<DbSet<Order>>();

    var customerName = Environment.GetEnvironmentVariable("CUSTOMER_NAME") ?? throw new ArgumentNullException("CUSTOMER_NAME");
    var customerEmail = Environment.GetEnvironmentVariable("CUSTOMER_EMAIL") ?? throw new ArgumentNullException("CUSTOMER_EMAIL");
    var customerPhoneNumber = Convert.ToInt32(Environment.GetEnvironmentVariable("CUSTOMER_PHONE_NUMBER") ?? throw new ArgumentNullException("CUSTOMER_PHONE_NUMBER"));
    var customerAddress = Environment.GetEnvironmentVariable("CUSTOMER_ADDRESS") ?? throw new ArgumentNullException("CUSTOMER_ADDRESS");
    var restaurantName = Environment.GetEnvironmentVariable("RESTAURANT_NAME") ?? throw new ArgumentNullException("RESTAURANT_NAME");

    var orderId = Guid.NewGuid();
    var order = new Order(customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName)
    {
        Id = orderId,
        TotalPrice = 200
    };
    order.AddOrderLine(new OrderLine(Guid.NewGuid(), orderId, 100, 2, "Pizza"));

    var orderList = new List<Order> { order }.AsQueryable();

    ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.Provider);
    ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.Expression);
    ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.ElementType);
    ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orderList.GetEnumerator());

    dbContextMock.Setup(db => db.Set<Order>()).Returns(ordersDbSetMock.Object);

    var result = controller.GetOrder(orderId);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var orderDto = Assert.IsType<OrderDto>(okResult.Value);
    Assert.Equal(orderId, orderDto.Id);
    Assert.Equal(customerName, orderDto.CustomerName);
    Assert.Equal(customerEmail, orderDto.CustomerEmail);
    Assert.Equal(customerPhoneNumber, orderDto.CustomerPhoneNumber);
    Assert.Equal(customerAddress, orderDto.CustomerAddress);
    Assert.Equal(restaurantName, orderDto.RestaurantName);
    Assert.Equal(200, orderDto.TotalPrice);
    Assert.Single(orderDto.OrderLines);
}

    [Fact]
    public void GetOrder_NonExistingOrderId_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        var dbContextMock = new Mock<OrderingContext>(new DbContextOptions<OrderingContext>());
        var ordersDbSetMock = new Mock<DbSet<Order>>();
        var orderList = new List<Order>().AsQueryable();

        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.Provider);
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.Expression);
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.ElementType);
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orderList.GetEnumerator());

        dbContextMock.Setup(db => db.Set<Order>()).Returns(ordersDbSetMock.Object);

        var orderServiceMock = new Mock<IOrderService>();
        var controller = new OrderController(dbContextMock.Object, orderServiceMock.Object);

        var result = controller.GetOrder(orderId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not found", notFoundResult.Value);
    }
}