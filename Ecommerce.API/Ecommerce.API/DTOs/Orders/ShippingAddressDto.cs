namespace Ecommerce.API.DTOs.Orders
{
    public class ShippingAddressDto
    {
        public string Address1 { get; set; } = default!;
        public string? Address2 { get; set; }
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
        public int ZipCode { get; set; }
    }

}
