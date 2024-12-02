using Microsoft.AspNetCore.Mvc;
using Ordering.API.Repositories;
namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;


        public OrderController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _orderRepository.GetByIdAsync(id, CancellationToken.None).Result;

            return Ok(order);
        }
    }
}