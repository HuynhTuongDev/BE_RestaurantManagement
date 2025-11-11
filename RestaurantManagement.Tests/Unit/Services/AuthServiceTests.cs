using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Infrastructure.Services.UserServices;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Application.Services.System;

namespace RestaurantManagement.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockJwtService.Object,
            _mockLogger.Object);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FullName = "New User",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>(), "Access"))
            .Returns("mock-jwt-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Registration successful");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User!.Email.Should().Be("newuser@example.com");
        result.User.FullName.Should().Be("New User");
        result.User.Role.Should().Be("Customer");

        _mockUserRepository.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(It.IsAny<User>(), "Access"), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FullName = "User"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");

        _mockUserRepository.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FullName = "Test User"
        };

        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("error");
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            FullName = "Test User",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            IsDeleted = false
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>(), "Access"))
            .Returns("mock-jwt-token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Login successful");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();

        _mockUserRepository.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(user, "Access"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            FullName = "Test User",
            Role = UserRole.Customer
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithDeletedUser_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "deleted@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 1,
            Email = "deleted@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsDeleted = true
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Account has been deleted");
    }

    [Fact]
    public async Task LoginAsync_WithSuspendedUser_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "suspended@example.com",
            Password = "Password123!"
        };

        var user = new User
        {
            Id = 1,
            Email = "suspended@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Status = UserStatus.Suspended,
            IsDeleted = false
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Account has been suspended");
    }

    #endregion

    #region GetUserProfileAsync Tests

    [Fact]
    public async Task GetUserProfileAsync_WithValidUserId_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            FullName = "Test User",
            Phone = "1234567890",
            Address = "123 Main St",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetUserProfileAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Email.Should().Be("user@example.com");
        result.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetUserProfileAsync_WithInvalidUserId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.GetUserProfileAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateProfileAsync Tests

    [Fact]
    public async Task UpdateProfileAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FullName = "Updated Name",
            Phone = "9876543210",
            Address = "456 New St"
        };

        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            FullName = "Old Name",
            Phone = "1234567890",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.UpdateProfileAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Profile updated successfully");
        result.User!.FullName.Should().Be("Updated Name");
        result.User.Phone.Should().Be("9876543210");

        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithInvalidUserId_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FullName = "Updated Name"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.UpdateProfileAsync(999, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123!"),
            FullName = "Test User"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ChangePasswordAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Password changed successfully");

        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ShouldReturnFailure()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        var user = new User
        {
            Id = 1,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ChangePasswordAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Current password is incorrect");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidUserId_ShouldReturnFailure()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword",
            ConfirmNewPassword = "NewPassword"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.ChangePasswordAsync(999, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    #endregion
}
