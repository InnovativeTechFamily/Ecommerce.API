namespace Ecommerce.API.DTOs.User
{
    public class UpdateUserPasswordDto
    {
        public string OldPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }

}
