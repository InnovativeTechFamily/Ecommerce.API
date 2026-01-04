namespace Ecommerce.API.DTOs.User
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserResponseDto? User { get; set; }
        public string? Token { get; set; }
    }
}
