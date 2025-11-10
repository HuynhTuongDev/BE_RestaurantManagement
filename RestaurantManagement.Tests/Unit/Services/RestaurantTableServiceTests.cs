using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Infrastructure.Services;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Tests.Unit.Services;

public class RestaurantTableServiceTests
{
    private readonly Mock<IRestaurantTableRepository> _mockTableRepository;
    private readonly Mock<ILogger<RestaurantTableService>> _mockLogger;
    private readonly RestaurantTableService _tableService;

    public RestaurantTableServiceTests()
    {
        _mockTableRepository = new Mock<IRestaurantTableRepository>();
        _mockLogger = new Mock<ILogger<RestaurantTableService>>();
        _tableService = new RestaurantTableService(_mockTableRepository.Object, _mockLogger.Object);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTable()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            TableNumber = 5,
            Seats = 4,
            Status = TableStatus.Available,
            Location = "Main Hall"
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);

        // Act
        var result = await _tableService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TableNumber.Should().Be(5);
        result.Seats.Should().Be(4);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockTableRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((RestaurantTable?)null);

        // Act
        var result = await _tableService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTables()
    {
        // Arrange
        var tables = new List<RestaurantTable>
        {
            new RestaurantTable { Id = 1, TableNumber = 1, Seats = 2, Status = TableStatus.Available },
            new RestaurantTable { Id = 2, TableNumber = 2, Seats = 4, Status = TableStatus.Reserved }
        };

        _mockTableRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tables);

        // Act
        var result = await _tableService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    #endregion

    #region SearchAsync Tests

    [Fact]
    public async Task SearchAsync_WithValidTableNumber_ShouldReturnMatchingTables()
    {
        // Arrange
        var tables = new List<RestaurantTable>
        {
            new RestaurantTable
            {
                Id = 1,
                TableNumber = 5,
                Seats = 4,
                Status = TableStatus.Available
            }
        };

        _mockTableRepository.Setup(x => x.SearchAsync(5))
            .ReturnsAsync(tables);

        // Act
        var result = await _tableService.SearchAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().TableNumber.Should().Be(5);
    }

    [Fact]
    public async Task SearchAsync_WithInvalidTableNumber_ShouldReturnEmptyList()
    {
        // Act
        var result = await _tableService.SearchAsync(0);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_WithNegativeNumber_ShouldReturnEmptyList()
    {
        // Act
        var result = await _tableService.SearchAsync(-1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidData_ShouldReturnCreatedTable()
    {
        // Arrange
        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 10,
            Seats = 6,
            Location = "VIP Section"
        };

        _mockTableRepository.Setup(x => x.AddAsync(It.IsAny<RestaurantTable>()))
            .Returns(Task.CompletedTask)
            .Callback<RestaurantTable>(t => t.Id = 1);

        // Act
        var result = await _tableService.AddAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.TableNumber.Should().Be(10);
        result.Seats.Should().Be(6);
        result.Status.Should().Be(TableStatus.Available);

        _mockTableRepository.Verify(x => x.AddAsync(It.IsAny<RestaurantTable>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithInvalidTableNumber_ShouldThrowException()
    {
        // Arrange
        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 0,
            Seats = 4
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _tableService.AddAsync(dto));
    }

    [Fact]
    public async Task AddAsync_WithInvalidSeats_ShouldThrowException()
    {
        // Arrange
        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 5,
            Seats = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _tableService.AddAsync(dto));
    }

    [Fact]
    public async Task AddAsync_WithTooManySeats_ShouldThrowException()
    {
        // Arrange
        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 5,
            Seats = 25 // More than 20
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _tableService.AddAsync(dto));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldComplete()
    {
        // Arrange
        var existingTable = new RestaurantTable
        {
            Id = 1,
            TableNumber = 5,
            Seats = 4,
            Status = TableStatus.Available
        };

        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 6,
            Seats = 6,
            Location = "Updated Location"
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingTable);
        _mockTableRepository.Setup(x => x.UpdateAsync(It.IsAny<RestaurantTable>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tableService.UpdateAsync(1, dto);

        // Assert
        _mockTableRepository.Verify(x => x.UpdateAsync(It.Is<RestaurantTable>(t =>
            t.TableNumber == 6 && t.Seats == 6
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentTable_ShouldThrowException()
    {
        // Arrange
        var dto = new RestaurantTableCreateDto
        {
            TableNumber = 5,
            Seats = 4
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((RestaurantTable?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _tableService.UpdateAsync(999, dto));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldComplete()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            TableNumber = 5
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);
        _mockTableRepository.Setup(x => x.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        await _tableService.DeleteAsync(1);

        // Assert
        _mockTableRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentTable_ShouldThrowException()
    {
        // Arrange
        _mockTableRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((RestaurantTable?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _tableService.DeleteAsync(999));
    }

    #endregion

    #region ReserveAsync Tests

    [Fact]
    public async Task ReserveAsync_WithAvailableTable_ShouldReturnTrue()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            TableNumber = 5,
            Status = TableStatus.Available
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);
        _mockTableRepository.Setup(x => x.UpdateAsync(It.IsAny<RestaurantTable>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tableService.ReserveAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockTableRepository.Verify(x => x.UpdateAsync(It.Is<RestaurantTable>(t => 
            t.Status == TableStatus.Reserved)), Times.Once);
    }

    [Fact]
    public async Task ReserveAsync_WithUnavailableTable_ShouldReturnFalse()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            Status = TableStatus.Occupied
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);

        // Act
        var result = await _tableService.ReserveAsync(1);

        // Assert
        result.Should().BeFalse();
        _mockTableRepository.Verify(x => x.UpdateAsync(It.IsAny<RestaurantTable>()), Times.Never);
    }

    [Fact]
    public async Task ReserveAsync_WithNonExistentTable_ShouldReturnFalse()
    {
        // Arrange
        _mockTableRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((RestaurantTable?)null);

        // Act
        var result = await _tableService.ReserveAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region CancelReservationAsync Tests

    [Fact]
    public async Task CancelReservationAsync_WithReservedTable_ShouldReturnTrue()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            TableNumber = 5,
            Status = TableStatus.Reserved
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);
        _mockTableRepository.Setup(x => x.UpdateAsync(It.IsAny<RestaurantTable>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tableService.CancelReservationAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockTableRepository.Verify(x => x.UpdateAsync(It.Is<RestaurantTable>(t => 
            t.Status == TableStatus.Available)), Times.Once);
    }

    [Fact]
    public async Task CancelReservationAsync_WithNonReservedTable_ShouldReturnFalse()
    {
        // Arrange
        var table = new RestaurantTable
        {
            Id = 1,
            Status = TableStatus.Available
        };

        _mockTableRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(table);

        // Act
        var result = await _tableService.CancelReservationAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelReservationAsync_WithNonExistentTable_ShouldReturnFalse()
    {
        // Arrange
        _mockTableRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((RestaurantTable?)null);

        // Act
        var result = await _tableService.CancelReservationAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
