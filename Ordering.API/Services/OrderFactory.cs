using Ordering.API.RequestDTOs;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Entities;

namespace Ordering.API.Services
{
    public static class OrderFactory
    {
        public static Order CreateOrderFromCart(CartDto cartDto, string customerName, string customerEmail, int customerPhoneNumber, string customerAddress, string restaurantName)
        {
            var order = new Order
            {
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhoneNumber = customerPhoneNumber,
                CustomerAddress = customerAddress,
                RestaurantName = restaurantName,
                TotalPrice = cartDto.TotalPrice
            };

            foreach (var orderLine in cartDto.CartItems.Select(cartItem => new OrderLine
                     {
                         Dish_Id = cartItem.Dish.Id,
                         DishName = cartItem.Dish.Name,
                         Quantity = cartItem.Quantity,
                         Price = cartItem.Dish.Price,
                         Order_Id = order.Id
                     }))
            {
                order.AddOrderLine(orderLine);
            }

            return order;
        }
    }
}