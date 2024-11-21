using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string key, Order value);
}