using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.User
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Please enter your name!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(4, ErrorMessage = "Password should be greater than 4 characters")]
        public string Password { get; set; }

        public string? Avatar { get; set; } // Base64 string for image
    }
}
