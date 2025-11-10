namespace RestaurantManagement.Application.Services.System;

using RestaurantManagement.Domain.Entities;

/// <summary>
/// Email service interface for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send password reset email to user
    /// </summary>
    /// <param name="user">User to send email to</param>
    /// <returns>Task</returns>
    Task SendResetPasswordEmail(User user);

    /// <summary>
    /// Send welcome email to new user
    /// </summary>
    /// <param name="user">New user</param>
    /// <returns>Task</returns>
    Task SendWelcomeEmail(User user);

    /// <summary>
    /// Send verification email
    /// </summary>
    /// <param name="user">User to verify</param>
    /// <param name="verificationToken">Verification token</param>
    /// <returns>Task</returns>
    Task SendVerificationEmail(User user, string verificationToken);
}
