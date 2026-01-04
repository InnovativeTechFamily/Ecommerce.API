namespace Ecommerce.API.DTOs.Shop
{
    public class CreateSellerDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Avatar { get; set; } = null;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
    }

    public class AvatarDto
    {
        public string Base64Data { get; set; }
        public string PublicId { get; set; } = default!;
        public string Url { get; set; } = default!;
    }

}
