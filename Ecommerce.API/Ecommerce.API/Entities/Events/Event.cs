using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Event
{
    public class Event
    {
        [Key]
        public string Id { get; set; } = NewId();   // "e_xxx"

        public static string NewId() => $"e_{Guid.CreateVersion7()}";

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
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "Please enter your event product stock!")]
        public int Stock { get; set; }

        [Required]
        public string ShopId { get; set; } = default!;

        /// <summary>
        /// Stored as JSON (optional)
        /// </summary>
        public string? Shop { get; set; }

        public int SoldOut { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Media (Images / Videos)
        public ICollection<Media> Media { get; set; } = new List<Media>();
    }
}
