using Ecommerce.API.Entities.Products;
using Ecommerce.API.Entities.Users;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.API.Entities.Shops
{
    public class Shop
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please enter your shop name!")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Please enter your shop email address")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        // Store hash here (equivalent to mongoose password field)
        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(6, ErrorMessage = "Password should be greater than 6 characters")]
        public string PasswordHash { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        public string Address { get; set; } = default!;

        [Required]
        public string PhoneNumber { get; set; } = default!; // use string to keep leading zeros etc.

        public string Role { get; set; } = "Seller";

        // avatar: { public_id, url }
        public string AvatarPublicId { get; set; } = default!;
        public string AvatarUrl { get; set; } = default!;

        [Required]
        public int ZipCode { get; set; }

        // withdrawMethod: flexible object -> store as JSON string or another table; here string
        public string? WithdrawMethodJson { get; set; }

        public decimal AvailableBalance { get; set; } = 0;

        public List<ShopTransaction> Transactions { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTime { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();


        // -------- Methods (like schema.methods) --------

        // Hash and set password (equivalent to pre("save") + bcrypt.hash)
        public void SetPassword(string plainPassword)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword); // bcrypt
        }

        // Compare password (equivalent to comparePassword)
        public bool ComparePassword(string enteredPassword)
        {
            if (string.IsNullOrEmpty(PasswordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(enteredPassword, PasswordHash);
        }

        // Get JWT token (equivalent to getJwtToken)
        public string GetJwtToken(string secretKey, TimeSpan expiresIn)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                // Node: { id: this._id } -> claim "id"
                new Claim("id", Id.ToString())
            }),
                Expires = DateTime.UtcNow.Add(expiresIn),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
