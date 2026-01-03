using Ecommerce.API.Utils;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities
{
    public class Media
    {
        [Key]
        public string Id { get; set; } = NewId();   // "m_xxx"

        public static string NewId() => $"m_{Guid.CreateVersion7()}";

        [Required]
        public string PublicId { get; set; } = default!; // Cloudinary public_id

        [Required]
        public string Url { get; set; } = default!;      // Cloudinary secure_url

        [Required]
        public string Folder { get; set; } = CloudinaryFolders.Products;

        public string? FileName { get; set; }
        // public string? MimeType { get; set; }

        // Entity ownership (simpler than OwnerType + OwnerId)
        public string? EntityType { get; set; }  // "Product", "User", "Shop"
        public string? EntityId { get; set; }    // "p_xxx", "u_xxx", "s_xxx"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
