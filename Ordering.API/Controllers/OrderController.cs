using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ordering.API.DTOs;
using Ordering.Domain.Entities;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderingContext _dbContext;

        public OrderController(OrderingContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _dbContext.Orders
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.DishName)
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
        public IActionResult CreateOrder([FromBody] CreateOrderDto? createOrderDto)
        {
            if (createOrderDto == null)
                return BadRequest("Invalid order data.");

            var order = new Order(createOrderDto.CustomerName, createOrderDto.CustomerEmail,
                createOrderDto.CustomerPhoneNumber, createOrderDto.CustomerAddress, createOrderDto.RestaurantName);
                
            foreach (var orderLineDto in createOrderDto.OrderLines)
            {
                var dish = _dbContext.Dishes.FirstOrDefault(d => d.Name == orderLineDto.DishName);
                if (dish == null)
                    return NotFound($"Dish '{orderLineDto.DishName}' not found.");

                var orderLine = new OrderLine(dish.Id, order.Id, dish.Price, orderLineDto.Quantity);
                order.AddOrderLine(orderLine);
            }

            order.CalculateTotalPrice();
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

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

        [HttpPost("{id}/annul")]
        public IActionResult AnnulOrder(Guid id)
        {
            var order = _dbContext.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found");

            // Simulate a "dice roll" for the annulment
            var random = new Random();
            var canAnnul = random.Next(0, 2) == 0;  // 50% chance to annul

            if (canAnnul)
            {
                // Remove all order lines as the annulment was successful
                _dbContext.OrderLines.RemoveRange(order.OrderLines);
                _dbContext.Orders.Remove(order);
                _dbContext.SaveChanges();

                return Ok("Order annulled successfully.");
            }
            else
            {
                return BadRequest("You were not able to annul the order in time.");
            }
        }
    }
}
