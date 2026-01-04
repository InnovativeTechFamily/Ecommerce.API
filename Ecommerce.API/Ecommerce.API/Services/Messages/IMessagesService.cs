using Ecommerce.API.DTOs.Chats;
using Ecommerce.API.Entities.Chats;

namespace Ecommerce.API.Services.Messages
{
    public interface IMessagesService
    {
        Task<Message> CreateMessageAsync(CreateMessageDto dto);
        Task<IReadOnlyList<Message>> GetMessagesByConversationIdAsync(string conversationId);
    }
}
