namespace Ecommerce.API.DTOs.Chats
{
    public class CreateMessageDto
    {
        public string ConversationId { get; set; } = default!;
        public string Text { get; set; } = default!;
        public string Sender { get; set; } = default!;
        public string? ImagesBase64 { get; set; } // base64 image data
    }

}
