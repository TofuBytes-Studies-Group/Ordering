using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ordering.Api.Controllers;
using Ordering.API.RequestDTOs;
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
}