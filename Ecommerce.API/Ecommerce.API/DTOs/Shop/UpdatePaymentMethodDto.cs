namespace Ecommerce.API.DTOs.Shop
{
    public class UpdatePaymentMethodDto
    {
        public string WithdrawMethodJson { get; set; } = default!;
        // JSON string like: '{"method": "paypal", "account": "seller@example.com"}'
    }

}
