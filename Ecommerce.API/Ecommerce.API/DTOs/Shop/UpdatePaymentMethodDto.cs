namespace Ecommerce.API.DTOs.Shop
{
    public class UpdatePaymentMethodDto
    {
        public object WithdrawMethodJson { get; set; } = default!;
        // JSON string like: '{"method": "paypal", "account": "seller@example.com"}'
    }

}
