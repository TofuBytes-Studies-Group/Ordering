using Moq;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure.Kafka;

namespace Ordering.UnitTests.Services;

public class KafkaProducerServiceTest
{
    [Fact]
    public async Task ProduceOrderAsync_WithValidOrder_ShouldReturnTrue()
    {
        // Arrange
        var order = new Order("Ikit Kugelblitz", "Forme@email.email", 1, "Hole","Yummyplace1");
        var kafkaProducer = new Mock<IKafkaProducer>();
        var kafkaProducerService = new KafkaProducerService(kafkaProducer.Object);
        
        kafkaProducer.Setup(x => x.ProduceAsync(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        await kafkaProducerService.ProduceOrderAsync(order);

        // Assert
        kafkaProducer.Verify(x => x.ProduceAsync("topic", "Order created", It.Is<Order>(order => order.CustomerName == "Ikit Kugelblitz")), Times.Once);
        
    }
}