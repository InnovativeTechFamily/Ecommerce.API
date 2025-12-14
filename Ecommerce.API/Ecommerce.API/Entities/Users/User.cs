using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Users
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter your name!")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email!")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(4, ErrorMessage = "Password should be greater than 4 characters")]
        public string PasswordHash { get; set; }

        public string? PhoneNumber { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

        [StringLength(50)]
        public string Role { get; set; } = "user";

        public virtual Avatar? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordTokenExpiry { get; set; }

        public bool IsActive { get; set; } = false;

        public string? ActivationToken { get; set; }

        public DateTime? ActivationTokenExpiry { get; set; }
    }
}
