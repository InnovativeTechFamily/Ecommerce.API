using Ecommerce.API.Exceptions;

namespace Ecommerce.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await SendEmailAsync(toEmail, subject, message, false);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml)
        {
            try
            {
                // Option 1: SendGrid (Recommended for production)
                await SendEmailWithSendGrid(toEmail, subject, body, isHtml);

                // Option 2: SMTP (Alternative)
                // await SendEmailWithSmtp(toEmail, subject, body, isHtml);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
                throw new EmailServiceException($"Failed to send email: {ex.Message}", ex);
            }
        }

        public async Task SendActivationEmailAsync(string toEmail, string name, string activationToken)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var activationUrl = $"{frontendUrl}/activation/{activationToken}";

            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Activate Your Account</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 30px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; 
                                 color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; 
                                 color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Our E-commerce Store!</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {name},</h2>
                            <p>Thank you for registering with us. To activate your account, please click the button below:</p>
                            <p>
                                <a href='{activationUrl}' class='button'>Activate Account</a>
                            </p>
                            <p>Or copy and paste this link in your browser:</p>
                            <p><code>{activationUrl}</code></p>
                            <p>This activation link will expire in 24 hours.</p>
                            <p>If you didn't create an account, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>Best regards,<br>The E-commerce Team</p>
                            <p>&copy; {DateTime.UtcNow.Year} E-commerce Store. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, "Activate Your Account - E-commerce Store", htmlBody, true);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string name, string resetToken)
        {
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/reset-password/{resetToken}";

            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .button {{ background-color: #007bff; color: white; padding: 10px 20px; 
                                 text-decoration: none; border-radius: 5px; }}
                    </style>
                </head>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello {name},</p>
                    <p>We received a request to reset your password. Click the link below to reset it:</p>
                    <p><a href='{resetUrl}' class='button'>Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you didn't request a password reset, please ignore this email.</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, "Password Reset Request - E-commerce Store", htmlBody, true);
        }

        private async Task SendEmailWithSendGrid(string toEmail, string subject, string body, bool isHtml)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("SendGrid API Key is not configured. Email will not be sent.");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject,
                isHtml ? null : body,
                isHtml ? body : null);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Body.ReadAsStringAsync();
                _logger.LogError($"SendGrid email failed: {errorResponse}");
                throw new EmailServiceException($"SendGrid failed with status: {response.StatusCode}");
            }
        }

        private async Task SendEmailWithSmtp(string toEmail, string subject, string body, bool isHtml)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }


}
