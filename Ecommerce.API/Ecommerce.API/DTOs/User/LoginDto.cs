using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.User
{
	public class LoginDto
	{

		[Required(ErrorMessage = "Please enter your email!")]
		[EmailAddress]
		public string Email { get; set; }

		[Required(ErrorMessage = "Please enter your password")]
		[MinLength(4, ErrorMessage = "Password should be greater than 4 characters")]
		public string Password { get; set; }
	}
}
