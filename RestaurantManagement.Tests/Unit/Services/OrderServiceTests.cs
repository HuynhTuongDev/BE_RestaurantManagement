using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Infrastructure.Services;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Tests.Unit.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        _orderService = new OrderService(_mockOrderRepository.Object, _mockLogger.Object);
    }

    #region CreateOrderAsync Tests

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new OrderCreateRequest
        {
            UserId = 1,
            TableId = 1,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { MenuItemId = 1, Quantity = 2 },
                new OrderItemRequest { MenuItemId = 2, Quantity = 1 }
            }
        };

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.CreateOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Order created successfully");
        result.Order.Should().NotBeNull();
        result.Order!.UserId.Should().Be(1);
        result.Order.TableId.Should().Be(1);
        result.Order.Status.Should().Be(OrderStatus.Pending);

        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var request = new OrderCreateRequest
        {
            UserId = 1,
            TableId = 1,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { MenuItemId = 1, Quantity = 2 }
            }
        };

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _orderService.CreateOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Failed to create order");
    }

    #endregion

    #region GetAllOrdersAsync Tests

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                UserId = 1,
                TableId = 1,
                Status = OrderStatus.Pending,
                OrderTime = DateTime.UtcNow,
                OrderDetails = new List<OrderDetail>()
            },
            new Order
            {
                Id = 2,
                UserId = 2,
                TableId = 2,
                Status = OrderStatus.Completed,
                OrderTime = DateTime.UtcNow,
                OrderDetails = new List<OrderDetail>()
            }
        };

        _mockOrderRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Orders.Should().HaveCount(2);

        _mockOrderRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetOrderByIdAsync Tests

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            TableId = 1,
            Status = OrderStatus.Pending,
            OrderTime = DateTime.UtcNow,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    MenuItemId = 1,
                    Quantity = 2,
                    Price = 10.00m,
                    MenuItem = new MenuItem { Name = "Pizza" }
                }
            }
        };

        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().MenuItemName.Should().Be("Pizza");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenUserIdNotMatch_ShouldReturnNull()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            TableId = 1,
            OrderDetails = new List<OrderDetail>()
        };

        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderByIdAsync(1, userId: 2);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateOrderAsync Tests

    [Fact]
    public async Task UpdateOrderAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var existingOrder = new Order
        {
            Id = 1,
            UserId = 1,
            TableId = 1,
            Status = OrderStatus.Pending,
            OrderDetails = new List<OrderDetail>()
        };

        var request = new OrderUpdateRequest
        {
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { MenuItemId = 1, Quantity = 3 }
            }
        };

        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(existingOrder);
        _mockOrderRepository.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.UpdateOrderAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Order updated successfully");

        _mockOrderRepository.Verify(x => x.UpdateAsync(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderAsync_WithNonExistentOrder_ShouldReturnFailure()
    {
        // Arrange
        var request = new OrderUpdateRequest
        {
            Items = new List<OrderItemRequest>()
        };

        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.UpdateOrderAsync(999, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Order not found");
    }

    [Fact]
    public async Task UpdateOrderAsync_WhenOrderNotPending_ShouldReturnFailure()
    {
        // Arrange
        var existingOrder = new Order
        {
            Id = 1,
            Status = OrderStatus.Completed,
            OrderDetails = new List<OrderDetail>()
        };

        var request = new OrderUpdateRequest
        {
            Items = new List<OrderItemRequest>()
        };

        _mockOrderRepository.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(existingOrder);

        // Act
        var result = await _orderService.UpdateOrderAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Only pending orders can be updated");
    }

    #endregion

    #region CancelOrderAsync Tests

    [Fact]
    public async Task CancelOrderAsync_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            Status = OrderStatus.Pending
        };

        _mockOrderRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);
        _mockOrderRepository.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.CancelOrderAsync(1, userId: 1, isCustomerRequest: true);

        // Assert
        result.Should().BeTrue();
        _mockOrderRepository.Verify(x => x.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Cancelled)), Times.Once);
    }

    [Fact]
    public async Task CancelOrderAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockOrderRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.CancelOrderAsync(999, null, false);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenCustomerNotOwner_ShouldReturnFalse()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            Status = OrderStatus.Pending
        };

        _mockOrderRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.CancelOrderAsync(1, userId: 2, isCustomerRequest: true);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenOrderNotPending_ShouldReturnFalse()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            Status = OrderStatus.Completed
        };

        _mockOrderRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.CancelOrderAsync(1, userId: 1, isCustomerRequest: true);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetOrderStatusAsync Tests

    [Fact]
    public async Task GetOrderStatusAsync_WithValidId_ShouldReturnStatus()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            Status = OrderStatus.Completed
        };

        _mockOrderRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderStatusAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task GetOrderStatusAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockOrderRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.GetOrderStatusAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderStatusAsync_WhenUserIdNotMatch_ShouldReturnNull()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = 1,
            Status = OrderStatus.Pending
        };

        _mockOrderRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderStatusAsync(1, userId: 2);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region SearchOrdersAsync Tests

    [Fact]
    public async Task SearchOrdersAsync_WithValidKeyword_ShouldReturnMatchingOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                UserId = 1,
                TableId = 1,
                Status = OrderStatus.Pending,
                OrderDetails = new List<OrderDetail>()
            }
        };

        _mockOrderRepository.Setup(x => x.SearchByKeywordAsync("1"))
            .ReturnsAsync(orders);

        // Act
        var result = await _orderService.SearchOrdersAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Orders.Should().HaveCount(1);
    }

    #endregion
}
