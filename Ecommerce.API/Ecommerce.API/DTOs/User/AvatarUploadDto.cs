using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.User
{
    public class AvatarUploadDto
    {
        [Required]
        public string ImageData { get; set; } // Base64 string or URL
    }
}
