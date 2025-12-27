namespace Ecommerce.API.DTOs.Shop
{
    // Data class to hold decoded token info
    public class ShopActivationData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string AvatarPublicId { get; set; } = default!;
        public string AvatarUrl { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public int ZipCode { get; set; }
    }

}
