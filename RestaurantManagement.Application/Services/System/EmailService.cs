using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Application.Services.System
{
    /// <summary>
    /// Email service implementation
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jWTService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration, 
            IJwtService jWTService, 
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _jWTService = jWTService;
            _logger = logger;
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        public async Task SendResetPasswordEmail(User user)
        {
            var token = _jWTService.GenerateToken(user, "Reset");
            var clientUrl = _configuration["AppSettings:ClientUrl"];
            var resetLink = $"{clientUrl}/reset-password/{token}";
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
            var templatePath = Path.Combine(projectRoot, "RestaurantManagement.Application", "Templates", "ResetPasswordTemplate.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);

            emailTemplate = emailTemplate
                .Replace("{{UserName}}", user.FullName ?? "User")
                .Replace("{{ResetLink}}", resetLink);

            await SendEmailAsync(user.Email, "Reset Your Password", emailTemplate);
        }

        /// <summary>
        /// Send welcome email to new user
        /// </summary>
        public async Task SendWelcomeEmail(User user)
        {
            var emailBody = $@"
                <html>
                <body>
                    <h2>Welcome {user.FullName}!</h2>
                    <p>Thank you for registering at our Restaurant Management System.</p>
                    <p>We're excited to have you on board!</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, "Welcome to Restaurant Management", emailBody);
        }

        /// <summary>
        /// Send verification email
        /// </summary>
        public async Task SendVerificationEmail(User user, string verificationToken)
        {
            var clientUrl = _configuration["AppSettings:ClientUrl"];
            var verificationLink = $"{clientUrl}/verify-email/{verificationToken}";
            
            var emailBody = $@"
                <html>
                <body>
                    <h2>Verify Your Email</h2>
                    <p>Hello {user.FullName},</p>
                    <p>Please click the link below to verify your email address:</p>
                    <a href='{verificationLink}'>Verify Email</a>
                    <p>This link will expire in 24 hours.</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, "Verify Your Email Address", emailBody);
        }

        /// <summary>
        /// Helper method to send email
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Restaurant Management", _configuration["SmtpSettings:Username"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _configuration["SmtpSettings:Server"],
                        int.Parse(_configuration["SmtpSettings:Port"]!),
                        SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(
                        _configuration["SmtpSettings:Username"],
                        _configuration["SmtpSettings:Password"]);

                    await client.SendAsync(message);
                    
                    _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
