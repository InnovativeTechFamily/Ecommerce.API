namespace Ecommerce.API.DTOs.Orders
{
    public class CreateOrderRequestDto
    {
        public List<CartItemDto> Cart { get; set; } = new();
        public ShippingAddressDto ShippingAddress { get; set; } = default!;
        public OrderUserDto User { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public PaymentInfoDto PaymentInfo { get; set; } = new();
    }

}
