/*using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ordering.Api.Controllers;
using Ordering.API.Repositories;
using Ordering.API.RequestDTOs;
using Ordering.API.ResponseDTOs;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure;

namespace Ordering.APITests;

public class OrderControllerTest
{
    private readonly OrderingContext _dbContext;
    private readonly Mock<IValidator<CartDto>> _validatorMock;
    private readonly Mock<OrderService> _orderServiceMock;
    private readonly OrderController _controller;

    public OrderControllerTest()
    {
        var options = new DbContextOptionsBuilder<OrderingContext>()
            .UseInMemoryDatabase(databaseName: "Orderdbtest")
            .Options;
        _dbContext = new OrderingContext(options);
        _validatorMock = new Mock<IValidator<CartDto>>();
        _orderServiceMock = new Mock<OrderService>(new Mock<IOrderRepository>().Object, new Mock<IKafkaProducerService>().Object);
        _controller = new OrderController(_orderServiceMock.Object, new OrderRepository(_dbContext), _validatorMock.Object);
    }

    [Fact]
    public async Task CreateOrder_ValidCartDto_ReturnsCreatedAtActionResult()
    {
        var cartDto = new CartDto
        {
            CustomerUserName = "john_doe",
            CartItems = new List<CartItemDto>
            {
                new CartItemDto
                {
                    //Dish = new DishDto { Id = Guid.NewGuid(), Name = "Pizza", Price = 100 },
                    Quantity = 2,
                    SumPrice = 200
                }
            },
            TotalPrice = 200
        };

        var customerName = "Pete Zar-riah Firenze";
        var customerEmail = "gabbagool@gogglemail.com";
        var customerPhoneNumber = 12345678;
        var customerAddress = "123 Fake Street";
        var restaurantName = "genereic pizza place";

        var result = await _controller.CreateOrder(cartDto, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var orderDto = Assert.IsType<OrderDto>(createdResult.Value);
        Assert.Equal(cartDto.CartItems.Count, orderDto.OrderLines.Count);
    }

    [Fact]
    public async Task CreateOrder_NullCartDto_ReturnsBadObjectRequest()
    {
        //arrange
        var validation = new ValidationResult();
        CartDto cartDto = null;
        _validatorMock.Setup(v => v.ValidateAsync(cartDto,default)).ReturnsAsync(validation);
        
        var customerName = "Pete Zar-riah Firenze";
        var customerEmail = "gabbagool@gogglemail.com";
        var customerPhoneNumber = 12345678;
        var customerAddress = "123 Fake Street";
        var restaurantName = "genereic pizza place";

        var result = await _controller.CreateOrder(cartDto, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetOrder_ExistingOrderId_ReturnsOrderDto()
    {
        var dbContextMock = new Mock<OrderingContext>(new DbContextOptions<OrderingContext>());
        var ordersDbSetMock = new Mock<DbSet<Order>>();

       

        var orderId = Guid.NewGuid();
        var order = new Order()
        {
            Id = orderId,
            TotalPrice = 200,
            CustomerId = customerId,
            RestaurantId = restaurantId,
        };
        order.AddOrderLine(new OrderLine(Guid.NewGuid(), orderId, 100, 2, "Pizza"));

        var orderList = new List<Order> { order }.AsQueryable();

        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.Provider);
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.Expression);
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.ElementType);
        using var enumerator = orderList.GetEnumerator();
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        dbContextMock.Setup(db => db.Set<Order>()).Returns(ordersDbSetMock.Object);

        var result = _controller.GetOrder(orderId);

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
        using var enumerator = orderList.GetEnumerator();
        ordersDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        dbContextMock.Setup(db => db.Set<Order>()).Returns(ordersDbSetMock.Object);

        var result = _controller.GetOrder(orderId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not found", notFoundResult.Value);
    }
}*/