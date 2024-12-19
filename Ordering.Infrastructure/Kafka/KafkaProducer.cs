using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Kafka
{
    public class KafkaProducer : IDisposable, IKafkaProducer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaProducer> _logger;

        // The Confluent.Kafka IProducer interface that provides serialization for the Key and Value
        //   -  The Key represents a unique identifier for the message, used for partitioning. Messages with the 
        //      same key are sent to the same partition, helping maintain order within that key’s data stream (e.g., all messages 
        //      from the same user or transaction ID). Can be null, then a random partition will be selected.
        //   -  The Value is the main content or payload of the message that holds the data you want to send. It's 
        //      generally the core information being transferred between producers and consumers. 
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Kafka producerconfigs using the Confluent.Kafka ProducerConfig object
            var config = new ProducerConfig
            {
                // We use the Kafka configs we set in appsettings.json
                // We can add more configs than bootstrapservers, but it might not be necessary
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };

            // We build the producer with the specified configs
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        
        // Constructor for testing purposes
        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger, IProducer<string, string> producer)
        {
            _configuration = configuration;
            _logger = logger;
            _producer = producer;
        }

        public async Task ProduceAsync(string topic, string key, Order value)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                // Construct the message to be sent with the key-value pair (same types as the producer expects).
                var message = new Message<string, string> { Key = key, Value = json };

                // ProduceAsync sends the message to Kafka.
                // The result contains metadata about the message that we can assign to a var if interested
                var deliveryResult = await _producer.ProduceAsync(topic, message);

                // We can log the deliveryResult fx
                _logger.LogInformation($"Message sent to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError($"Error producing message: {e.Error.Reason}");
            }
        }

        // Dispose method ensures the producer is closed and resources are released when done.
        // Should be disposed when done by default because of Singleton use in Program.cs but to be sure:
        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
