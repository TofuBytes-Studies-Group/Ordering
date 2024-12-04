using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Interfaces;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _orderRepository.GetByIdAsync(id, CancellationToken.None).Result;

            return Ok(order);
        }
    }
}