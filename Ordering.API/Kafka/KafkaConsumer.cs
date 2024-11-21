using System.Text.Json;
using Confluent.Kafka;
using Ordering.API.RequestDTOs;
using Ordering.API.Services;
using Ordering.Domain.Interfaces;

namespace Ordering.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "groupId",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("topic");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer is running.");
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));

                        if (consumeResult != null)
                        {
                            var message = consumeResult.Message.Value;
                            var cartDto = JsonSerializer.Deserialize<CartDto>(message);

                            if (cartDto != null)
                            {
                                using var scope = _serviceScopeFactory.CreateScope();
                                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                                // Replace these with actual values or retrieve them from a suitable source
                                string customerName = "John Doe";
                                string customerEmail = "john.doe@example.com";
                                int customerPhoneNumber = 1234567890;
                                string customerAddress = "123 Main St";
                                string restaurantName = "Best Restaurant";

                                var order = OrderFactory.CreateOrderFromCart(cartDto, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);
                                await orderService.CreateOrderAsync(order, stoppingToken);

                                _logger.LogInformation($"Order created for User: {cartDto.Username}");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Error consuming Kafka message: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer stopping gracefully.");
            }
            finally
            {
                _consumer.Close();
                _logger.LogInformation("Kafka consumer has stopped.");
            }
        }
    }
}