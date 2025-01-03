using System.Text.Json;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.API.Kafka;
using Ordering.API.RequestDTOs;
using Ordering.API.Services;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure.Kafka;
using Xunit;

namespace Ordering.SystemTests
{
    public class OrderSystemTests : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<KafkaProducer>> _mockProducerLogger;
        private readonly Mock<ILogger<KafkaConsumer>> _mockConsumerLogger;
        private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
        private readonly Mock<IValidator<CartDto>> _mockValidator;
        private readonly KafkaProducer _kafkaProducer;
        private readonly KafkaConsumer _kafkaConsumer;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IKafkaProducerService> _mockKafkaProducerService;

        public OrderSystemTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockProducerLogger = new Mock<ILogger<KafkaProducer>>();
            _mockConsumerLogger = new Mock<ILogger<KafkaConsumer>>();
            _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            _mockValidator = new Mock<IValidator<CartDto>>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockKafkaProducerService = new Mock<IKafkaProducerService>();
            _mockOrderService = new Mock<IOrderService>();

            _mockConfiguration.Setup(config => config["Kafka:BootstrapServers"]).Returns("localhost:9092");

            _kafkaProducer = new KafkaProducer(_mockConfiguration.Object, _mockProducerLogger.Object);
            _kafkaConsumer = new KafkaConsumer(_mockConfiguration.Object, _mockConsumerLogger.Object, _mockServiceScopeFactory.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task EndToEnd_OrderProcessing()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = Guid.NewGuid(),
                CustomerUsername = "john_doe",
                RestaurantId = Guid.NewGuid(),
                TotalPrice = 200
            };

            var cartDto = new CartDto
            {
                CustomerId = order.CustomerId,
                CustomerUsername = order.CustomerUsername,
                RestaurantId = order.RestaurantId,
                TotalPrice = order.TotalPrice,
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        Dish = new DishDto { Id = Guid.NewGuid(), Price = 100 },
                        Quantity = 2
                    }
                }
            };

            var message = JsonSerializer.Serialize(cartDto);
            var consumeResult = new ConsumeResult<string, string>
            {
                Message = new Message<string, string> { Key = "test-key", Value = message }
            };

            var mockScope = new Mock<IServiceScope>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider.Setup(sp => sp.GetService(typeof(IOrderService))).Returns(_mockOrderService.Object);
            mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
            _mockServiceScopeFactory.Setup(ssf => ssf.CreateScope()).Returns(mockScope.Object);

            _mockOrderService.Setup(os => os.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _kafkaProducer.ProduceAsync("order.accepted", order.CustomerUsername, order);
            _kafkaProducer.Dispose(); // Dispose of the producer after producing the message
            _kafkaConsumer.ConsumeResult = consumeResult;

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5)); // Cancel after 5 seconds

            var consumerTask = _kafkaConsumer.StartAsync(cts.Token);

            // Wait for the consumer to process the message
            await Task.Delay(TimeSpan.FromSeconds(6));

            // Assert
            _mockOrderService.Verify(os => os.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);

            // Ensure the consumer task completes
            await consumerTask;
        }

        public void Dispose()
        {
            _kafkaConsumer.Dispose();
        }
    }
}