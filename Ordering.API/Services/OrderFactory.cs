using Ordering.API.RequestDTOs;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;

namespace Ordering.API.Services
{
    public static class OrderFactory
    {
        public static Order CreateOrderFromCart(CartDto cartDto)
        {
            if (cartDto.CartItems == null)
            {
                throw new ArgumentNullException(nameof(cartDto.CartItems), "Cart items cannot be null");
            }

            var orderLines = cartDto.CartItems.Select(cartItem => new OrderLine
            {
                DishId = cartItem.Dish.Id,
                Quantity = cartItem.Quantity,
                Price = cartItem.Dish.Price
            }).ToList();

            var order = new Order
            {
                CustomerId = cartDto.CustomerId,
                RestaurantId = cartDto.RestaurantId,
                CustomerUsername = cartDto.CustomerUsername,
                TotalPrice = cartDto.TotalPrice,
                OrderLines = orderLines
            };

            return order;
        }
    }
}