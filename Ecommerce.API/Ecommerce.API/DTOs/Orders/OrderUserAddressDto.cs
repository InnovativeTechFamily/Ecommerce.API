namespace Ecommerce.API.DTOs.Orders
{
    public class OrderUserAddressDto
    {
        public string Address1 { get; set; } = default!;
        public string? Address2 { get; set; }
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
        public int ZipCode { get; set; }
        public string AddressType { get; set; } = default!;
    }

}
