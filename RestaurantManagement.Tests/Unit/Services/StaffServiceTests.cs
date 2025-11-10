using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Infrastructure.Services.UserServices;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Tests.Unit.Services;

public class StaffServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<StaffService>> _mockLogger;
    private readonly StaffService _staffService;

    public StaffServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<StaffService>>();
        _staffService = new StaffService(_mockUserRepository.Object, _mockLogger.Object);
    }

    #region CreateStaffAsync Tests

    [Fact]
    public async Task CreateStaffAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new StaffCreateRequest
        {
            FullName = "John Staff",
            Email = "staff@example.com",
            Password = "Password123!",
            Phone = "1234567890",
            Address = "123 Office St",
            Position = "Manager",
            HireDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _staffService.CreateStaffAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Staff created successfully");
        result.Staff.Should().NotBeNull();
        result.Staff!.FullName.Should().Be("John Staff");
        result.Staff.Position.Should().Be("Manager");

        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateStaffAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = new StaffCreateRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FullName = "Staff"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _staffService.CreateStaffAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task CreateStaffAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var request = new StaffCreateRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FullName = "Test Staff"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _staffService.CreateStaffAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("An error occurred while creating staff");
    }

    #endregion

    #region GetAllStaffAsync Tests

    [Fact]
    public async Task GetAllStaffAsync_ShouldReturnAllStaff()
    {
        // Arrange
        var staff = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "Staff 1",
                Email = "staff1@example.com",
                Role = UserRole.Staff,
                StaffProfile = new StaffProfile
                {
                    Position = "Manager",
                    HireDate = DateTime.UtcNow
                }
            },
            new User
            {
                Id = 2,
                FullName = "Staff 2",
                Email = "staff2@example.com",
                Role = UserRole.Staff,
                StaffProfile = new StaffProfile
                {
                    Position = "Waiter",
                    HireDate = DateTime.UtcNow
                }
            }
        };

        _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Staff))
            .ReturnsAsync(staff);

        // Act
        var result = await _staffService.GetAllStaffAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Position.Should().Be("Manager");
    }

    [Fact]
    public async Task GetAllStaffAsync_WhenExceptionThrown_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Staff))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _staffService.GetAllStaffAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetStaffByIdAsync Tests

    [Fact]
    public async Task GetStaffByIdAsync_WithValidId_ShouldReturnStaff()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FullName = "John Staff",
            Email = "staff@example.com",
            Role = UserRole.Staff,
            StaffProfile = new StaffProfile
            {
                Position = "Manager",
                HireDate = DateTime.UtcNow
            }
        };

        _mockUserRepository.Setup(x => x.GetByIdWithProfileAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _staffService.GetStaffByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FullName.Should().Be("John Staff");
        result.Position.Should().Be("Manager");
    }

    [Fact]
    public async Task GetStaffByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdWithProfileAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _staffService.GetStaffByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStaffByIdAsync_WhenUserIsNotStaff_ShouldReturnNull()
    {
        // Arrange
        var customer = new User
        {
            Id = 1,
            Role = UserRole.Customer
        };

        _mockUserRepository.Setup(x => x.GetByIdWithProfileAsync(1))
            .ReturnsAsync(customer);

        // Act
        var result = await _staffService.GetStaffByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateStaffAsync Tests

    [Fact]
    public async Task UpdateStaffAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            Role = UserRole.Staff,
            StaffProfile = new StaffProfile
            {
                Position = "Old Position",
                HireDate = DateTime.UtcNow.AddYears(-1)
            }
        };

        var request = new StaffCreateRequest
        {
            FullName = "New Name",
            Email = "new@example.com",
            Position = "New Position",
            HireDate = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdWithProfileAsync(1))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.EmailExistsAsync("new@example.com"))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _staffService.UpdateStaffAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Staff updated successfully");
        result.Staff!.FullName.Should().Be("New Name");
    }

    [Fact]
    public async Task UpdateStaffAsync_WithNonExistentStaff_ShouldReturnFailure()
    {
        // Arrange
        var request = new StaffCreateRequest
        {
            FullName = "New Name"
        };

        _mockUserRepository.Setup(x => x.GetByIdWithProfileAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _staffService.UpdateStaffAsync(999, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Staff not found");
    }

    #endregion

    #region DeleteStaffAsync Tests

    [Fact]
    public async Task DeleteStaffAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var staff = new User
        {
            Id = 1,
            Role = UserRole.Staff
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(staff);
        _mockUserRepository.Setup(x => x.SoftDeleteUserAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _staffService.DeleteStaffAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStaffAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _staffService.DeleteStaffAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region SearchStaffAsync Tests

    [Fact]
    public async Task SearchStaffAsync_WithValidKeyword_ShouldReturnMatchingStaff()
    {
        // Arrange
        var staff = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "John Manager",
                Role = UserRole.Staff,
                StaffProfile = new StaffProfile
                {
                    Position = "Manager",
                    HireDate = DateTime.UtcNow
                }
            }
        };

        _mockUserRepository.Setup(x => x.SearchByKeywordAsync("john", UserRole.Staff))
            .ReturnsAsync(staff);

        // Act
        var result = await _staffService.SearchStaffAsync("john");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("John Manager");
    }

    [Fact]
    public async Task SearchStaffAsync_WhenExceptionThrown_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.SearchByKeywordAsync(It.IsAny<string>(), UserRole.Staff))
            .ThrowsAsync(new Exception("Search error"));

        // Act
        var result = await _staffService.SearchStaffAsync("test");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion
}
