using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Entities.Chats
{
    public class Conversation
    {
        [Key]
        public string Id { get; set; } = NewId();
        public static string NewId() => $"cv_{Guid.CreateVersion7()}"; // Changed from "c_"

        public string? GroupTitle { get; set; }
        public List<string> Members { get; set; } = new();
        public string? LastMessage { get; set; }
        public string? LastMessageId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
