using Ordering.Domain.Aggregates;

namespace Ordering.API.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceOrderAsync(Order order);
    }
}