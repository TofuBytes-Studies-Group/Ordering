using Moq;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Kafka;

namespace Ordering.UnitTests.Services;

public class KafkaProducerServiceTest
{
    [Fact]
    public async Task ProduceOrderAsync_WithValidOrder_ShouldReturnTrue()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = new Guid(),
            CustomerUsername = "Ikit Kugelblitz",
            Id = new Guid(),
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    Id = new Guid(),
                    OrderId = new Guid(),
                    DishId = new Guid(),
                    Quantity = 1,
                    Price = 10
                }
            },
            RestaurantId = new Guid(),
            TotalPrice = 10
        };
        var kafkaProducer = new Mock<IKafkaProducer>();
        var kafkaProducerService = new KafkaProducerService(kafkaProducer.Object);
        
        kafkaProducer.Setup(x => x.ProduceAsync(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        await kafkaProducerService.ProduceOrderAsync(order);

        // Assert
        kafkaProducer.Verify(x => x.ProduceAsync("order.accepted", order.CustomerUsername, It.Is<Order>(o => o.CustomerUsername == "Ikit Kugelblitz")), Times.Once);
        
    }
}