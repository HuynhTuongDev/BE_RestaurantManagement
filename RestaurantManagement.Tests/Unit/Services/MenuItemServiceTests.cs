namespace RestaurantManagement.Tests.Unit.Services;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Services;
using Xunit;

/// <summary>
/// Unit tests for MenuItemService
/// </summary>
public class MenuItemServiceTests
{
    private readonly Mock<IMenuItemRepository> _mockRepository;
    private readonly Mock<ILogger<MenuItemService>> _mockLogger;
    private readonly MenuItemService _service;

    public MenuItemServiceTests()
    {
        _mockRepository = new Mock<IMenuItemRepository>();
        _mockLogger = new Mock<ILogger<MenuItemService>>();
        _service = new MenuItemService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMenuItem()
    {
        // Arrange
        var menuItemId = 1;
        var expectedMenuItem = new MenuItem
        {
            Id = menuItemId,
            Name = "Pizza Margherita",
            Description = "Classic cheese pizza",
            Price = 150000,
            Category = "Pizza"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(menuItemId))
            .ReturnsAsync(expectedMenuItem);

        // Act
        var result = await _service.GetByIdAsync(menuItemId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedMenuItem);
        result.Name.Should().Be("Pizza Margherita");
        result.Price.Should().Be(150000);

        _mockRepository.Verify(r => r.GetByIdAsync(menuItemId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var menuItemId = 999;

        _mockRepository
            .Setup(r => r.GetByIdAsync(menuItemId))
            .ReturnsAsync((MenuItem)null);

        // Act
        var result = await _service.GetByIdAsync(menuItemId);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(r => r.GetByIdAsync(menuItemId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMenuItems()
    {
        // Arrange
        var expectedMenuItems = new List<MenuItem>
        {
            new() { Id = 1, Name = "Pizza Margherita", Price = 150000, Category = "Pizza" },
            new() { Id = 2, Name = "Pasta Carbonara", Price = 120000, Category = "Pasta" },
            new() { Id = 3, Name = "Salad Fresh", Price = 80000, Category = "Salad" }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedMenuItems);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(m => m.Name == "Pizza Margherita");
        result.Should().Contain(m => m.Name == "Pasta Carbonara");
        result.Should().Contain(m => m.Name == "Salad Fresh");

        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithValidMenuItem_AddsSuccessfully()
    {
        // Arrange
        var newMenuItem = new MenuItem
        {
            Name = "New Pizza",
            Description = "New pizza description",
            Price = 160000,
            Category = "Pizza"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<MenuItem>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(newMenuItem);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Pizza");
        result.Price.Should().Be(160000);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MenuItem>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithInvalidPrice_ThrowsArgumentException()
    {
        // Arrange
        var invalidMenuItem = new MenuItem
        {
            Name = "Invalid Pizza",
            Description = "Invalid pizza",
            Price = -100,  // Invalid price
            Category = "Pizza"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(invalidMenuItem));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MenuItem>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var invalidMenuItem = new MenuItem
        {
            Name = null,  // Invalid: null name
            Description = "Invalid pizza",
            Price = 150000,
            Category = "Pizza"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(invalidMenuItem));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MenuItem>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WithKeyword_ReturnsFilteredResults()
    {
        // Arrange
        var keyword = "Pizza";
        var searchResults = new List<MenuItem>
        {
            new() { Id = 1, Name = "Pizza Margherita", Price = 150000, Category = "Pizza" },
            new() { Id = 4, Name = "Pizza Pepperoni", Price = 170000, Category = "Pizza" }
        };

        _mockRepository
            .Setup(r => r.SearchAsync(keyword))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.Name.Should().Contain("Pizza"));

        _mockRepository.Verify(r => r.SearchAsync(keyword), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesSuccessfully()
    {
        // Arrange
        var existingMenuItem = new MenuItem
        {
            Id = 1,
            Name = "Original Name",
            Description = "Original description",
            Price = 150000,
            Category = "Pizza"
        };

        var updatedMenuItem = new MenuItem
        {
            Id = 1,
            Name = "Updated Name",
            Description = "Updated description",
            Price = 160000,
            Category = "Pizza"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingMenuItem);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<MenuItem>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(updatedMenuItem);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MenuItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonexistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var menuItem = new MenuItem { Id = 999, Name = "Test", Price = 100000, Category = "Test" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((MenuItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(menuItem));

        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MenuItem>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        var menuItemId = 1;
        var existingMenuItem = new MenuItem { Id = menuItemId, Name = "To Delete", Price = 100000, Category = "Test" };

        _mockRepository
            .Setup(r => r.GetByIdAsync(menuItemId))
            .ReturnsAsync(existingMenuItem);

        _mockRepository
            .Setup(r => r.DeleteAsync(menuItemId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(menuItemId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(menuItemId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(menuItemId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonexistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var menuItemId = 999;

        _mockRepository
            .Setup(r => r.GetByIdAsync(menuItemId))
            .ReturnsAsync((MenuItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(menuItemId));

        _mockRepository.Verify(r => r.GetByIdAsync(menuItemId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
