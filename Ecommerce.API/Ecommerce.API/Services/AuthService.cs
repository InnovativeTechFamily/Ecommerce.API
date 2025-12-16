using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.API.Data;
using Ecommerce.API.DTOs.User;
using Ecommerce.API.Entities.Users;
using Ecommerce.API.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.API.Services
{

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
       // private readonly IEmailService _emailService;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration)//,
           // IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            //_emailService = emailService;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                throw new BadRequestException("User already exists");
            }

			
			var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
				PasswordHash =HashPassword(userDto.Password),
				CreatedAt = DateTime.UtcNow,
                Role = "user",
                IsActive = true
            };

            // Handle avatar if provided
            if (!string.IsNullOrEmpty(userDto.Avatar))
            {
                // Upload to Cloudinary or storage service
                var avatarResult = await UploadAvatar(userDto.Avatar);
                user.Avatar = new Avatar
                {
                    Id = Guid.NewGuid(),
                    PublicId = avatarResult.PublicId,
                    Url = avatarResult.Url,
                    UserId = user.Id
                };
            }

            // Generate activation token
           // user.ActivationToken = await GenerateActivationToken(user);
           // user.ActivationTokenExpiry = DateTime.UtcNow.AddDays(1);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Send activation email
            //await SendActivationEmail(user);

            return MapToUserResponseDto(user);
        }

		public static string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		public async Task<string> GenerateActivationToken(User user)
        {
            var token = Guid.NewGuid().ToString();
            var tokenHash = HashPassword(token);
            return tokenHash;
        }

        private async Task<(string PublicId, string Url)> UploadAvatar(string base64Image)
        {
            // Implement Cloudinary upload logic here
            // For now, returning mock data
            return (PublicId: Guid.NewGuid().ToString(), Url: "https://example.com/avatar.jpg");
        }

        private async Task SendActivationEmail(User user)
        {
            var activationUrl = $"{_configuration["AppSettings:FrontendUrl"]}/activation/{user.ActivationToken}";

            var emailBody = $@"
                <h1>Activate Your Account</h1>
                <p>Hello {user.Name},</p>
                <p>Please click the link below to activate your account:</p>
                <a href='{activationUrl}'>{activationUrl}</a>
                <p>This link will expire in 24 hours.</p>";

            //await _emailService.SendEmailAsync(
            //    user.Email,
            //    "Activate your account",
            //    emailBody);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

		public static bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}

		public async Task<bool> VerifyLoginUser(UserLoginDto userLoginDto)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);

			if (user == null)
				throw new BadRequestException("User does not exist");

			if (!user.IsActive)
				throw new BadRequestException("Account is inactive");

			bool isPasswordValid = VerifyPassword(
				userLoginDto.Password,
				user.PasswordHash
			);

			if (!isPasswordValid)
				throw new BadRequestException("Invalid email or password");

			return true;
		}
		private UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Avatar = user.Avatar != null ? new AvatarDto
                {
                    PublicId = user.Avatar.PublicId,
                    Url = user.Avatar.Url
                } : null
            };
        }
    }
}
