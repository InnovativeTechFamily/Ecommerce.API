using Ecommerce.API.DTOs;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ecommerce.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.Server)
                || string.IsNullOrWhiteSpace(_settings.Port)
                || string.IsNullOrWhiteSpace(_settings.Username)
                || string.IsNullOrWhiteSpace(_settings.Password)
                || string.IsNullOrWhiteSpace(_settings.SenderEmail))
            {
                _logger.LogError("EmailSettings missing required values.");
                throw new InvalidOperationException("Email settings are not configured.");
            }

            if (!int.TryParse(_settings.Port, out var port))
            {
                _logger.LogError("EmailSettings:Port is not a valid integer: {Port}", _settings.Port);
                throw new InvalidOperationException("Email port is invalid.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_settings.SenderName ?? "", _settings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw; // let middleware handle / return 500 with logged details
            }
        }
    }
}
