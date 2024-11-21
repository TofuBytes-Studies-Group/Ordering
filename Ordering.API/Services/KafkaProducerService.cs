
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

        public async void DoStuff()
        {
            // Brug KafkaProducer
            await _kafkaProducer.ProduceAsync("topic", "Virker", new Order());
        }
        
        public async Task ProduceOrderAsync(Order order)
        {
            await _kafkaProducer.ProduceAsync("topic", "Order created", order);
        }
    }
}
