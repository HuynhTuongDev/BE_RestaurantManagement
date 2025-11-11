using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Infrastructure.Services.UserServices;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Tests.Unit.Services;

public class CustomerServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _customerService = new CustomerService(_mockUserRepository.Object, _mockLogger.Object);
    }

    #region CreateCustomerAsync Tests

    [Fact]
    public async Task CreateCustomerAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _customerService.CreateCustomerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Customer created successfully");
        result.Customer.Should().NotBeNull();
        result.Customer!.FullName.Should().Be("John Doe");
        result.Customer.Email.Should().Be("john@example.com");

        _mockUserRepository.Verify(x => x.EmailExistsAsync(It.IsAny<string>()), Times.Once);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            FullName = "John Doe",
            Email = "existing@example.com",
            Phone = "1234567890"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.CreateCustomerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");

        _mockUserRepository.Verify(x => x.EmailExistsAsync("existing@example.com"), Times.Once);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithoutEmail_ShouldGenerateTempEmail()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            FullName = "Walk-In Customer",
            Email = null,
            Phone = "1234567890"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _customerService.CreateCustomerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Customer.Should().NotBeNull();
        result.Customer!.Email.Should().BeEmpty(); // Temp email is masked in DTO
    }

    [Fact]
    public async Task CreateCustomerAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            FullName = "John Doe",
            Email = "john@example.com"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _customerService.CreateCustomerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("An error occurred while creating customer");
    }

    #endregion

    #region GetAllCustomersAsync Tests

    [Fact]
    public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "Customer 1",
                Email = "customer1@example.com",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                FullName = "Customer 2",
                Email = "customer2@example.com",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Customer))
            .ReturnsAsync(customers);

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().FullName.Should().Be("Customer 1");

        _mockUserRepository.Verify(x => x.GetByRoleAsync(UserRole.Customer), Times.Once);
    }

    [Fact]
    public async Task GetAllCustomersAsync_WhenNoCustomers_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Customer))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCustomersAsync_WhenExceptionThrown_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Customer))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetCustomerByIdAsync Tests

    [Fact]
    public async Task GetCustomerByIdAsync_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new User
        {
            Id = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenUserIsNotCustomer_ShouldReturnNull()
    {
        // Arrange
        var staff = new User
        {
            Id = 1,
            FullName = "Staff User",
            Email = "staff@example.com",
            Role = UserRole.Staff
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(staff);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateCustomerAsync Tests

    [Fact]
    public async Task UpdateCustomerAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var existingCustomer = new User
        {
            Id = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        var request = new CustomerCreateRequest
        {
            FullName = "New Name",
            Email = "new@example.com",
            Phone = "9876543210"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCustomer);
        _mockUserRepository.Setup(x => x.EmailExistsAsync("new@example.com"))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _customerService.UpdateCustomerAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Customer updated successfully");
        result.Customer!.FullName.Should().Be("New Name");

        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WithNonExistentCustomer_ShouldReturnFailure()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            FullName = "New Name",
            Email = "new@example.com"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _customerService.UpdateCustomerAsync(999, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Customer not found");
    }

    [Fact]
    public async Task UpdateCustomerAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var existingCustomer = new User
        {
            Id = 1,
            FullName = "Old Name",
            Email = "old@example.com",
            Role = UserRole.Customer
        };

        var request = new CustomerCreateRequest
        {
            FullName = "New Name",
            Email = "existing@example.com"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCustomer);
        _mockUserRepository.Setup(x => x.EmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.UpdateCustomerAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
    }

    #endregion

    #region DeleteCustomerAsync Tests

    [Fact]
    public async Task DeleteCustomerAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var customer = new User
        {
            Id = 1,
            FullName = "John Doe",
            Role = UserRole.Customer
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);
        _mockUserRepository.Setup(x => x.SoftDeleteUserAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _customerService.DeleteCustomerAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockUserRepository.Verify(x => x.SoftDeleteUserAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _customerService.DeleteCustomerAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockUserRepository.Verify(x => x.SoftDeleteUserAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenUserIsNotCustomer_ShouldReturnFalse()
    {
        // Arrange
        var staff = new User
        {
            Id = 1,
            Role = UserRole.Staff
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(staff);

        // Act
        var result = await _customerService.DeleteCustomerAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region SearchCustomersAsync Tests

    [Fact]
    public async Task SearchCustomersAsync_WithValidKeyword_ShouldReturnMatchingCustomers()
    {
        // Arrange
        var customers = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john@example.com",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            }
        };

        _mockUserRepository.Setup(x => x.SearchByKeywordAsync("john", UserRole.Customer))
            .ReturnsAsync(customers);

        // Act
        var result = await _customerService.SearchCustomersAsync("john");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task SearchCustomersAsync_WhenExceptionThrown_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.SearchByKeywordAsync(It.IsAny<string>(), UserRole.Customer))
            .ThrowsAsync(new Exception("Search error"));

        // Act
        var result = await _customerService.SearchCustomersAsync("test");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion
}
