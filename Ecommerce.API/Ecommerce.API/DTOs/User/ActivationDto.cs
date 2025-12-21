using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.User
{
    public class ActivationDto
    {
        [Required]
        public string ActivationToken { get; set; }
    }
}
