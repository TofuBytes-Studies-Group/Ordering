/*using Moq;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;

namespace Ordering.APITests;

public class OrderServiceTest
{
    [Fact]
    public async Task CreateOrderAsync_ValidOrder_AddsOrderToRepository()
    {
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var kafkaProducerServiceMock = new Mock<IKafkaProducerService>();

        var orderService = new OrderService(orderRepositoryMock.Object, kafkaProducerServiceMock.Object);

        var order = new Order
        {
            CustomerId = new Guid(),
            RestaurantId = new Guid()
            
        };

        await orderService.CreateOrderAsync(order, CancellationToken.None);
        //  assert/verify
        orderRepositoryMock.Verify(repo => repo.AddAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        kafkaProducerServiceMock.Verify(service => service.ProduceOrderAsync(order), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_NullOrder_ThrowsArgumentNullException()
    {
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
        var orderService = new OrderService(orderRepositoryMock.Object, kafkaProducerServiceMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orderService.CreateOrderAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingOrderId_ReturnsOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerName = "john doe",
            CustomerEmail = "john@example.com",
            CustomerPhoneNumber = 12345678,
            CustomerAddress = "123 Street",
            RestaurantName = "Test Restaurant"
        };
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await orderRepositoryMock.Object.GetByIdAsync(orderId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingOrderId_ReturnsNull()
    {
        var orderId = Guid.NewGuid();
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(repo =>
            repo.GetByIdAsync(orderId,
                It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var result = await orderRepositoryMock.Object.GetByIdAsync(orderId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var orders = new List<Order>
        {
            new()
            {
                CustomerName = "john doe",
                CustomerEmail = "john@example.com",
                CustomerPhoneNumber = 12345678,
                CustomerAddress = "123 Street",
                RestaurantName = "Test Restaurant"
            },
            new()
            {
                CustomerName = "jane doe",
                CustomerEmail = "janesemail@online.dk",
                CustomerPhoneNumber = 12345678,
                CustomerAddress = "123 uptown street",
                RestaurantName = "Fancy Test Restaurant"
            }
        };
        var orderRepositoryMock = new Mock<IOrderRepository>();
        orderRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(orders);

        var result = await orderRepositoryMock.Object.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}*/