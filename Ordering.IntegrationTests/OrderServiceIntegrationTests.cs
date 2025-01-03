using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ordering.API.Repositories;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure;

namespace Ordering.IntegrationTests;

public class OrderServiceIntegrationTests
{
    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContext<OrderingContext>(options =>
        {
            options.UseInMemoryDatabase("testDB");
        });
        services.AddScoped<IOrderRepository, OrderRepository>();
        // Mock the Kafka producer service
        var mockKafkaProducerService = new Mock<IKafkaProducerService>();
        mockKafkaProducerService.Setup(k => k.ProduceOrderAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        services.AddSingleton(mockKafkaProducerService.Object);
        services.AddScoped<IOrderService, OrderService>();
        return services.BuildServiceProvider();
    }
    
    [Fact]
    public async Task CreateOrderAsync_SavesOrderAndProducesKafkaMessage()
    {
        var serviceProvider = GetServiceProvider();
        var orderService = serviceProvider.GetRequiredService<IOrderService>();

        var order = new Order
        {
            CustomerId = Guid.NewGuid(),
            CustomerUsername = "john_doe",
            RestaurantId = Guid.NewGuid(),
            TotalPrice = 200
        };

        await orderService.CreateOrderAsync(order, CancellationToken.None);
        
        var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
        var savedOrder = await orderRepository.GetByIdAsync(order.Id, CancellationToken.None);
        Assert.NotNull(savedOrder);
        Assert.Equal("john_doe", savedOrder.CustomerUsername);

    }
    
}