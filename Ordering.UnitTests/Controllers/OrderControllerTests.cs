using Microsoft.AspNetCore.Mvc;
using Moq;
using Ordering.Api.Controllers;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Interfaces;

namespace Ordering.UnitTests.Controllers;

    public class OrderControllerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _orderController = new OrderController(_mockOrderRepository.Object);
        }

        [Fact]
        public Task GetOrder_ReturnsOkResult_WithOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                CustomerId = Guid.NewGuid(),
                CustomerUsername = "john_doe",
                RestaurantId = Guid.NewGuid(),
                TotalPrice = 200
            };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = _orderController.GetOrder(orderId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(order, result.Value);
            return Task.CompletedTask;
        }

        [Fact]
        public Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null);

            // Act
            var result = _orderController.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            return Task.CompletedTask;
        }
    }