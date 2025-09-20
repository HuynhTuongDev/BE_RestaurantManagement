using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Application.Services.System
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(User user);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jWTService;
        public EmailService(IConfiguration configuration, IUserRepository userRepository, IJwtService jWTService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _jWTService = jWTService;
        }

        public async Task SendResetPasswordEmail(User user)
        {
            var token = _jWTService.GenerateToken(user,"Reset");
            var clientUrl = _configuration["AppSettings:ClientUrl"];
            var resetLink = $"{clientUrl}/reset-password/{token}";
            
            // Try to find template file in multiple possible locations
            var templatePath = FindTemplateFile("ResetPasswordTemplate.html");
            if (templatePath == null)
            {
                throw new FileNotFoundException("Email template file not found");
            }
            
            var emailTemplate = await File.ReadAllTextAsync(templatePath);


            emailTemplate = emailTemplate
                .Replace("{{UserName}}", user.FullName ?? "User")
                .Replace("{{ResetLink}}", resetLink);

            var message = new MimeMessage();
            var fromEmail = _configuration["SmtpSettings:FromEmail"];
            var fromName = _configuration["SmtpSettings:FromName"] ?? "Restaurant Management";
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", user.Email));
            message.Subject = "Reset Your Password";
            message.Body = new TextPart("html") { Text = emailTemplate };

            using (var client = new SmtpClient())
            {

                await client.ConnectAsync(_configuration["SmtpSettings:Server"],
                                  int.Parse(_configuration["SmtpSettings:Port"] ?? "587"),
                                  MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"]);

                try
                {
                    await client.SendAsync(message); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.ToString()}"); 
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }

        private string? FindTemplateFile(string templateFileName)
        {
            // Try multiple possible locations for the template file
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName),
                Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateFileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RestaurantManagement.Application", "Templates", templateFileName)
            };

            return possiblePaths.FirstOrDefault(File.Exists);
        }
    }
}
