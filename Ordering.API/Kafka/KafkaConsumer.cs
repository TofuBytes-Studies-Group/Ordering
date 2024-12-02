using System.Text.Json;
using Confluent.Kafka;
using FluentValidation;
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
        private readonly IValidator<CartDto> _validator;


        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger,
            IServiceScopeFactory serviceScopeFactory, IValidator<CartDto> validator)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _validator = validator;

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "groupId",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("create.order");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer is running.");
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
                        if (consumeResult is { Message: not null })
                        {
                            var message = consumeResult.Message.Value;
                            _logger.LogInformation($"Received Message: {message}");
                            var cartDto = JsonSerializer.Deserialize<CartDto>(message);
                            if (cartDto != null)
                            {
                                using (var scope = _serviceScopeFactory.CreateScope())
                                {
                                    await _validator.ValidateAsync(cartDto, stoppingToken);
                                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                                    var order = OrderFactory.CreateOrderFromCart(cartDto);
                                    await orderService.CreateOrderAsync(order, stoppingToken);
                                }

                                _logger.LogInformation($"Order created for User: {cartDto.CustomerUserName}");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Error consuming Kafka message: {ex.Message}");
                    }
                    catch (ArgumentNullException ex)
                    {
                        _logger.LogError($"Error deserializing Kafka message: {ex.Message}");
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