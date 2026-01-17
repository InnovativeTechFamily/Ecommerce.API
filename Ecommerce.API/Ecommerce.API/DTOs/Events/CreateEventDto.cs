using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public string Category { get; set; } = default!;

        public string? Tags { get; set; }

        public decimal? OriginalPrice { get; set; }

        [Required]
        public decimal DiscountPrice { get; set; }

        [Required]
        public int Stock { get; set; }

        //[Required] we are  use in by cookies 
        //public string ShopId { get; set; } = default!;

        [Required]
        public DateTime Start_Date { get; set; }

        [Required]
        public DateTime Finish_Date { get; set; }

        // Base64 images from frontend
        public List<string> Images { get; set; } = new();
    }
}
