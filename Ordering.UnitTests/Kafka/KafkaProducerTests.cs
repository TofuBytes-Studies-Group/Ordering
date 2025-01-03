using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure.Kafka;
using Xunit;

namespace Ordering.UnitTests.Kafka
{
    public class KafkaProducerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<KafkaProducer>> _mockLogger;
        private readonly Mock<IProducer<string, string>> _mockProducer;
        private readonly KafkaProducer _kafkaProducer;

        public KafkaProducerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<KafkaProducer>>();
            _mockProducer = new Mock<IProducer<string, string>>();

            _mockConfiguration.Setup(config => config["Kafka:BootstrapServers"]).Returns("localhost:9092");

            // Use the constructor for testing purposes
            _kafkaProducer = new KafkaProducer(_mockConfiguration.Object, _mockLogger.Object, _mockProducer.Object);
        }

        [Fact]
        public async Task ProduceAsync_SendsMessageToKafka()
        {
            // Arrange
            const string topic = "test-topic";
            const string key = "test-key";
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                CustomerUsername = "john_doe",
                RestaurantId = Guid.NewGuid(),
                TotalPrice = 200
            };

            var json = JsonSerializer.Serialize(order);
            var message = new Message<string, string> { Key = key, Value = json };

            // Mocking the producer's ProduceAsync method
            _mockProducer
                .Setup(p => p.ProduceAsync(topic, It.Is<Message<string, string>>(m => m.Key == key && m.Value == json), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<string, string>
                {
                    Topic = topic,
                    Partition = 0,
                    Offset = new Offset(0),
                    Status = PersistenceStatus.Persisted,
                    // No need to explicitly set Key/Value here since it isn't directly verified.
                });

            // Act
            await _kafkaProducer.ProduceAsync(topic, key, order);

            // Assert
            _mockProducer.Verify(p => p.ProduceAsync(topic, It.Is<Message<string, string>>(m => m.Key == key && m.Value == json), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact]
        public void Dispose_ClosesProducer()
        {
            // Act
            _kafkaProducer.Dispose();

            // Assert
            _mockProducer.Verify(p => p.Dispose(), Times.Once);
        }
    }
}