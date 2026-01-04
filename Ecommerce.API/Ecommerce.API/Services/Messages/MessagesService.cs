using Ecommerce.API.Data;
using Ecommerce.API.DTOs.Chats;
using Ecommerce.API.Entities.Chats;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services.Messages
{

    public class MessagesService : IMessagesService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICloudinaryService _cloudinary;

        public MessagesService(ApplicationDbContext db, ICloudinaryService cloudinary)
        {
            _db = db;
            _cloudinary = cloudinary;
        }

        public async Task<Message> CreateMessageAsync(CreateMessageDto dto)
        {
            MessageImage? images = null;

            // Upload image if provided
            if (!string.IsNullOrWhiteSpace(dto.ImagesBase64))
            {
                var uploadResult = await _cloudinary.UploadBase64ImageAsync(
                    dto.ImagesBase64,
                    "messages"
                );

                images = new MessageImage
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.ImageUrl
                };
            }

            var message = new Message
            {
                ConversationId = dto.ConversationId,
                Text = dto.Text,
                Sender = dto.Sender,
                Images = images
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            return message;
        }

        public async Task<IReadOnlyList<Message>> GetMessagesByConversationIdAsync(string conversationId)
        {
            return await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
