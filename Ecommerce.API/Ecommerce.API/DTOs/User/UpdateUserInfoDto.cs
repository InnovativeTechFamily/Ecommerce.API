namespace Ecommerce.API.DTOs.User
{
    public class UpdateUserInfoDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

}
