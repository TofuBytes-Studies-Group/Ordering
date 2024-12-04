
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure.Kafka;

namespace Ordering.API.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IKafkaProducer _kafkaProducer;
        public KafkaProducerService(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
        
        public async Task ProduceOrderAsync(Order order)
        {
            await _kafkaProducer.ProduceAsync("order.accepted", order.CustomerUsername, order);
        }
    }
}
