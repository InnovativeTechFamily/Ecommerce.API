namespace Ecommerce.API.DTOs.Chats
{
    public class CreateConversationDto
    {
        public string GroupTitle { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string SellerId { get; set; } = default!;
    }
}
