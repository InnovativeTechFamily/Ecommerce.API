namespace Ecommerce.API.Services
{
    public interface IOrderEmailService
    {
        Task SendOrderConfirmationAsync(string toEmail, string subject, string htmlBody);
    }
}
