using Ecommerce.API.Entities.Chats;

namespace Ecommerce.API.Services.Conversations
{
    public interface IConversationService
    {
        Task<Ecommerce.API.Entities.Chats.Conversation> CreateOrGetConversationAsync(string groupTitle, string userId, string sellerId);
        Task<IReadOnlyList<Conversation>> GetSellerConversationsAsync(string sellerId); // NEW
        Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(string userId); // NEW
        Task<Conversation> UpdateLastMessageAsync(string conversationId, string lastMessage, string lastMessageId); // NEW
    }
}
