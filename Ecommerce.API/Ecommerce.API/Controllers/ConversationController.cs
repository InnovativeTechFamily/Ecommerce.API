using Ecommerce.API.DTOs.Chats;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services.Conversations;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpPost("create-new-conversation")]
    public async Task<IActionResult> CreateNewConversation([FromBody] CreateConversationDto dto)
    {
        try
        {
            var conversation = await _conversationService
                .CreateOrGetConversationAsync(dto.GroupTitle, dto.UserId, dto.SellerId);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                conversation
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }

    }
    [HttpGet("get-all-conversation-seller/{id}")]
    [IsSeller]
    public async Task<IActionResult> GetSellerConversations(string id)
    {
        try
        {
            var conversations = await _conversationService.GetSellerConversationsAsync(id);
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                conversations
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }



    [HttpGet("get-all-conversation-user/{id}")]
    [IsAuthenticated]
    public async Task<IActionResult> GetUserConversations(string id)
    {
        try
        {
            var conversations = await _conversationService.GetUserConversationsAsync(id);
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                conversations
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }

    [HttpPut("update-last-message/{id}")]
    public async Task<IActionResult> UpdateLastMessage(string id, [FromBody] UpdateLastMessageDto dto)
    {
        try
        {
            var conversation = await _conversationService
                .UpdateLastMessageAsync(id, dto.LastMessage, dto.LastMessageId);

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                conversation
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }
}
