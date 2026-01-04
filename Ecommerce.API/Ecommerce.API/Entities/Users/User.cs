using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name!")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email!")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(4, ErrorMessage = "Password should be greater than 4 characters")]
        public string Password { get; set; }

        public string? PhoneNumber { get; set; }

        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();

        public string Role { get; set; } = "user";

        public Avatar? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordTokenExpiry { get; set; }

        // Hash and set password (equivalent to pre("save") + bcrypt.hash)
        public void SetPassword(string plainPassword)
        {
            Password = BCrypt.Net.BCrypt.HashPassword(plainPassword); // bcrypt
        }
        // Compare password (equivalent to comparePassword)
        public bool ComparePassword(string enteredPassword)
        {
            if (string.IsNullOrEmpty(Password))
                return false;

            return BCrypt.Net.BCrypt.Verify(enteredPassword, Password);
        }
    }
}
