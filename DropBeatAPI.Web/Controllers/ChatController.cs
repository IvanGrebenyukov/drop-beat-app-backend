using System.Security.Claims;
using DropBeatAPI.Core.DTOs.Chat;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly UserManager<User> _userManager;

    public ChatController(IChatService chatService, UserManager<User> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
    }

    [HttpPost("send")]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = GetUserId();
        var message = await _chatService.SendMessageAsync(userId, dto);
        return Ok(message);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(Guid chatId)
    {
        return Ok(await _chatService.GetMessagesAsync(chatId));
    }

    [HttpGet("private-chats")]
    public async Task<IActionResult> GetPrivateChats()
    {
        var userId = GetUserId();
        var chats = await _chatService.GetPrivateChatsAsync(userId);
        return Ok(chats);
    }

    [HttpGet("genres-chats")]
    public async Task<IActionResult> GetGenreChats()
    {
        var chats = await _chatService.GetGenreChatsAsync();
        return Ok(chats);
    }
    
    [HttpPost("create-private/{otherUserId}")]
    public async Task<ActionResult<Guid>> CreatePrivateChat(Guid otherUserId)
    {
        var currentUserId = GetUserId();
        var chatId = await _chatService.CreatePrivateChatAsync(currentUserId, otherUserId);
        return Ok(chatId);
    }
    
    [HttpGet("private-chat-id/{otherUserId}")]
    public async Task<ActionResult<Guid?>> GetPrivateChatId(Guid otherUserId)
    {
        var currentUserId = GetUserId();
        var chatId = await _chatService.GetPrivateChatIdAsync(currentUserId, otherUserId);
        if (chatId == null)
        {
            return Ok("");
        }
        return Ok(chatId);
    }
    
    [HttpGet("{chatId}/type")]
    public async Task<ActionResult<string>> GetChatType(Guid chatId)
    {
        var type = await _chatService.GetChatTypeAsync(chatId);
        return Ok(type);
    }
    
    private Guid GetUserId()
    {
        return Guid.Parse(_userManager.GetUserId(User));
    }
}
