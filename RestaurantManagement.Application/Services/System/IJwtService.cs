namespace RestaurantManagement.Application.Services.System;

/// <summary>
/// JWT service interface for token management
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate JWT token for user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <param name="type">Token type (Access, Reset, Verification)</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(Domain.Entities.User user, string type = "Access");

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <param name="type">Token type (Access, Reset, Verification)</param>
    /// <returns>ClaimsPrincipal if valid, null otherwise</returns>
    global::System.Security.Claims.ClaimsPrincipal? ValidateToken(string token, string type = "Access");

    /// <summary>
    /// Get user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID or null</returns>
    int? GetUserIdFromToken(string token);

    /// <summary>
    /// Check if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if expired</returns>
    bool IsTokenExpired(string token);
}
