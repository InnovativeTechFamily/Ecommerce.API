namespace Ecommerce.API.DTOs.Shop
{
    public class UpdateSellerInfoDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
    }

}
