using Ecommerce.API.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Ecommerce.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger, IConfiguration config)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _config = config;
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

        public async  Task SendWithdrawRequestEmailAsync(string email, string sellerName, decimal amount)
        {
            var subject = "Withdraw Request";
            var message = $"Hello {sellerName}, Your withdraw request of ${amount} is processing. It will take 3days to 7days to processing!";

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse(_config["Smtp:Mail"]));
            mailMessage.To.Add(MailboxAddress.Parse(email));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart(TextFormat.Plain) { Text = message };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]!), SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(_config["Smtp:Mail"], _config["Smtp:Password"]);
            await smtp.SendAsync(mailMessage);
            await smtp.DisconnectAsync(true);
        }
    }
}
