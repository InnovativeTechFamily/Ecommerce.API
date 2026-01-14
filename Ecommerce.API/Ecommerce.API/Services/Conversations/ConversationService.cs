using Ecommerce.API.Data;
using Ecommerce.API.Entities.Chats;
using Ecommerce.API.Utils;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.API.Services.Conversations
{

    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _db;

        public ConversationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Ecommerce.API.Entities.Chats.Conversation> CreateOrGetConversationAsync(string groupTitle, string userId, string sellerId)
        {
            // Check if conversation exists by groupTitle (like findOne({ groupTitle }))
            var existingConversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.GroupTitle == groupTitle);

            if (existingConversation != null)
            {
                return existingConversation;
            }

            // Create new conversation
            var conversation = new Ecommerce.API.Entities.Chats.Conversation
            {
                GroupTitle = groupTitle,
                Members = new List<string> { userId, sellerId }
            };

            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();

            return conversation;
        }
        public async Task<IReadOnlyList<Conversation>> GetSellerConversationsAsync(string sellerId)
        {
            return await _db.Conversations
                .Where(c => c.Members.Contains(sellerId)) // like members: { $in: [sellerId] }
                .OrderByDescending(c => c.UpdatedAt)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();
        }


        public async Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _db.Conversations
                .Where(c => c.Members.Contains(userId)) // same logic as seller
                .OrderByDescending(c => c.UpdatedAt)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conversation> UpdateLastMessageAsync(string conversationId, string lastMessage, string lastMessageId)
        {
            var conversation = await _db.Conversations.FindAsync(conversationId);
            if (conversation == null)
                throw new ErrorHandler("Conversation not found", 404);

            conversation.LastMessage = lastMessage;
            conversation.LastMessageId = lastMessageId;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return conversation;
        }



    }
}
