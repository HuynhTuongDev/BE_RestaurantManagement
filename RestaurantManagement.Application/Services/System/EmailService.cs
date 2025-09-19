using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace BackEnd.Service.ServiceImpl
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
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
            var templatePath = Path.Combine(projectRoot, "RestaurantManagement.Application", "Templates", "ResetPasswordTemplate.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);


            emailTemplate = emailTemplate
                .Replace("{{UserName}}", user.FullName ?? "User")
                .Replace("{{ResetLink}}", resetLink);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Restaurant", "lehuynhtuong9a2@gmail.com"));
            message.To.Add(new MailboxAddress("", user.Email));
            message.Subject = "Activate your account";
            message.Body = new TextPart("html") { Text = emailTemplate };

            using (var client = new SmtpClient())
            {

                await client.ConnectAsync(_configuration["SmtpSettings:Server"],
                                  int.Parse(_configuration["SmtpSettings:Port"]),
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
    }
}
