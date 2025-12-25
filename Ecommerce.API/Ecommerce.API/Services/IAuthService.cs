using Ecommerce.API.DTOs.User;
using Ecommerce.API.Entities.Users;
using System.Net.Mail;

namespace Ecommerce.API.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserDto userDto);
        Task<string> GenerateActivationToken(User user);
        string GenerateJwtToken(User user);
		Task<bool> VerifyLoginUser(LoginDto loginDto);
    }


}
