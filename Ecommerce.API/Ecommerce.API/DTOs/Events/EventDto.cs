using Ecommerce.API.DTOs.Shop;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.DTOs.Events
{
    public class EventDto
    {
        public string Id { get; set; } = default!;

        [Required(ErrorMessage = "Please enter your event product name!")]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Please enter your event product description!")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Please enter your event product category!")]
        [MaxLength(100)]
        public string Category { get; set; } = default!;

        [Required]
        public DateTime Start_Date { get; set; }

        [Required]
        public DateTime Finish_Date { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Running";

        public string? Tags { get; set; }

        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "Please enter your event product price!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "Please enter your event product stock!")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required]
        public string ShopId { get; set; } = default!;

        public int SoldOut { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public List<MediaDto> Images { get; set; } = new();

        public ShopDto? Shop { get; set; }
    }
    public class UpdateEventDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        public string? Tags { get; set; }

        public decimal? OriginalPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? DiscountPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; }

        public int? SoldOut { get; set; }
    }
    public class MediaDto
    {
        public string Id { get; set; } = default!;
        public string PublicId { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Folder { get; set; } = default!;
        public string? FileName { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
