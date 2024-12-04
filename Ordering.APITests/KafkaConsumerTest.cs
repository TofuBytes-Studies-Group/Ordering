using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.API.Kafka;
using Ordering.API.RequestDTOs;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;

namespace Ordering.APITests;

public class KafkaConsumerTest
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<KafkaConsumer>> _mockLogger;
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IConsumer<string, string>> _mockConsumer;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<IValidator<CartDto>> _validatorMock;

    public KafkaConsumerTest()
    {
        // Arrange
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<KafkaConsumer>>();
        _mockOrderService = new Mock<IOrderService>();
        _mockConsumer = new Mock<IConsumer<string, string>>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _validatorMock = new Mock<IValidator<CartDto>>();

        _mockConfiguration.Setup(config => config["Kafka:BootstrapServers"]).Returns("localhost:9092");

        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.Setup(scope => scope.ServiceProvider.GetService(typeof(IOrderService)))
            .Returns(_mockOrderService.Object);
        _mockServiceScopeFactory.Setup(factory => factory.CreateScope())
            .Returns(mockServiceScope.Object);
    }

    [Fact]
    public async Task ConsumeOrderMessage_ShouldCreateOrderAndValidateCart()
    {
        // Arrange
        var cartJson = "{\"CustomerUserName\":\"carol\",\"CartItems\":[{\"Dish\":{\"Id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"Name\":\"pizza\",\"Price\":120},\"Quantity\":4,\"SumPrice\":480},{\"Dish\":{\"Id\":\"9fa85f64-5717-4562-b3fc-2c963f66afa6\",\"Name\":\"burger\",\"Price\":90},\"Quantity\":1,\"SumPrice\":90}],\"TotalPrice\":570}";

        _mockConsumer
            .Setup(consumer => consumer.Consume(It.IsAny<TimeSpan>()))
            .Returns(new ConsumeResult<string, string>
            {
                Message = new Message<string, string>
                {
                    Key = "key",
                    Value = cartJson
                }
            });

        var kafkaConsumer = new KafkaConsumer(
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockServiceScopeFactory.Object,
            _validatorMock.Object
        );

        // Use reflection to set the private _consumer field
        typeof(KafkaConsumer)
            .GetField("_consumer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(kafkaConsumer, _mockConsumer.Object);

        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await kafkaConsumer.StartAsync(cts.Token);

        // Assert
        _mockOrderService.Verify(
            service => service.CreateOrderAsync(It.IsAny<Order>(),
                It.IsAny<CancellationToken>()), Times.Once);
        _validatorMock.Verify(
            validator => validator.ValidateAsync(It.IsAny<CartDto>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockConsumer.Verify(consumer => consumer.Subscribe("create.order"), Times.Once);
        _mockConsumer.Verify(consumer => consumer.Consume(It.IsAny<TimeSpan>()), Times.AtLeastOnce);
    }
}