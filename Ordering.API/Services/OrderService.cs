using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;

namespace Ordering.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IKafkaProducerService _kafkaProducerService;

    public OrderService(IOrderRepository orderRepository, IKafkaProducerService kafkaProducerService)
    {
        _orderRepository = orderRepository;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        // Add the order to the repository
        await _orderRepository.AddAsync(order, cancellationToken);
        await _kafkaProducerService.ProduceOrderAsync(order);
    }
    
    public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _orderRepository.GetByIdAsync(id, cancellationToken);
    }
}