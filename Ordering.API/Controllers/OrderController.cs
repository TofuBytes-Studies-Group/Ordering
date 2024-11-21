using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ordering.API.DTOs;
using Ordering.API.RequestDTOs;
using Ordering.API.Services;
using Ordering.Domain.Interfaces;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderingContext _dbContext;
        private readonly IOrderService _orderService;

        public OrderController(OrderingContext dbContext, IOrderService orderService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _dbContext.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found");

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhoneNumber = order.CustomerPhoneNumber,
                CustomerAddress = order.CustomerAddress,
                RestaurantName = order.RestaurantName,
                TotalPrice = order.TotalPrice,
                OrderLines = order.OrderLines.Select(ol => new OrderLineDto
                {
                    Id = ol.Id,
                    DishName = ol.DishName,
                    Price = ol.Price,
                    Quantity = ol.Quantity
                }).ToList()
            };

            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] CartDto cartDto, [FromQuery] string customerName, [FromQuery] string customerEmail, [FromQuery] int customerPhoneNumber, [FromQuery] string customerAddress, [FromQuery] string restaurantName)
        {
            var order = OrderFactory.CreateOrderFromCart(cartDto, customerName, customerEmail, customerPhoneNumber, customerAddress, restaurantName);

            await _orderService.CreateOrderAsync(order, CancellationToken.None);

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhoneNumber = order.CustomerPhoneNumber,
                CustomerAddress = order.CustomerAddress,
                RestaurantName = order.RestaurantName,
                TotalPrice = order.TotalPrice,
                OrderLines = order.OrderLines.Select(ol => new OrderLineDto
                {
                    Id = ol.Id,
                    DishName = ol.DishName,
                    Price = ol.Price,
                    Quantity = ol.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }
    }
}