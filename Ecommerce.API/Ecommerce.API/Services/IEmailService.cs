namespace Ecommerce.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendEmailAsync(string toEmail, string subject, string htmlBody, bool isHtml);
        Task SendActivationEmailAsync(string toEmail, string name, string activationToken);
        Task SendPasswordResetEmailAsync(string toEmail, string name, string resetToken);
    }
}
