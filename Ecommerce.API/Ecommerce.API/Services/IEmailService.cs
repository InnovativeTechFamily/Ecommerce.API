namespace Ecommerce.API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendWithdrawRequestEmailAsync(string email, string sellerName, decimal amount);
    }
}
