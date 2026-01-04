using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Chats
{
    public class Message
    {
        [Key]
        public string Id { get; set; } = NewId();
        public static string NewId() => $"msg_{Guid.CreateVersion7()}";

        [Required]
        public string ConversationId { get; set; } = default!;

        public string? Text { get; set; }

        [Required]
        public string Sender { get; set; } = default!; // "u_xxx" or "s_xxx"

        public MessageImage? Images { get; set; } // optional image

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class MessageImage
    {
        public string PublicId { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
