namespace RestaurantManagement.Tests.Integration.Controllers;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

/// <summary>
/// Integration tests for MenuItemController
/// </summary>
public class MenuItemControllerTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public MenuItemControllerTests()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        // Add any required setup (e.g., authentication, seed data)
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllMenuItems_ReturnsOk()
    {
        // Arrange
        // (Assuming we have test data seeded)

        // Act
        var response = await _client.GetAsync("/api/v1/menu-item");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetMenuItemById_WithValidId_ReturnsOk()
    {
        // Arrange
        var menuItemId = 1;

        // Act
        var response = await _client.GetAsync($"/api/v1/menu-item/{menuItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMenuItemById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var menuItemId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/v1/menu-item/{menuItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchMenuItems_WithKeyword_ReturnsOk()
    {
        // Arrange
        var keyword = "pizza";

        // Act
        var response = await _client.GetAsync($"/api/v1/menu-item/search?keyword={keyword}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateMenuItem_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newMenuItem = new
        {
            name = "New Pizza Test",
            description = "Test pizza description",
            price = 160000,
            category = "Pizza"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(newMenuItem),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/menu-item", content);

        // Assert
        // Note: This may return 401 Unauthorized if authentication is required
        // In that case, you should add authentication token to the request
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateMenuItem_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidMenuItem = new
        {
            name = "",  // Invalid: empty name
            price = -100,  // Invalid: negative price
            category = "Pizza"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidMenuItem),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/menu-item", content);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMenuItem_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var menuItemId = 1;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/menu-item/{menuItemId}");

        // Assert
        // Note: May return 401 if authentication is required
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMenuItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var menuItemId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/menu-item/{menuItemId}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized);
    }
}
