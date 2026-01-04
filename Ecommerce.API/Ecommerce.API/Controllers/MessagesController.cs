using Ecommerce.API.DTOs.Chats;
using Ecommerce.API.Services.Messages;
using Ecommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService _messagesService;

    public MessagesController(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    [HttpPost("create-new-message")]
    public async Task<IActionResult> CreateNewMessage([FromBody] CreateMessageDto dto)
    {
        try
        {
            var message = await _messagesService.CreateMessageAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }

    [HttpGet("get-all-messages/{id}")]
    public async Task<IActionResult> GetAllMessages(string id)
    {
        try
        {
            var messages = await _messagesService.GetMessagesByConversationIdAsync(id);
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                messages
            });
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(ex.Message, 500);
        }
    }
}
