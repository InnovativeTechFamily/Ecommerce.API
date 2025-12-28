namespace Ecommerce.API.DTOs.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; }
        public AvatarDto? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserAddressDto> Addresses { get; set; }
    }

}
