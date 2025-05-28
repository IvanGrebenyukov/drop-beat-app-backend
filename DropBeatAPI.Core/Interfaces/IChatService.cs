using DropBeatAPI.Core.DTOs.Chat;
using DropBeatAPI.Core.Entities;

namespace DropBeatAPI.Core.Interfaces;

public interface IChatService
{
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto);
    Task<List<MessageDto>> GetMessagesAsync(Guid chatId);
    Task<List<UserChatDto>> GetPrivateChatsAsync(Guid userId);
    Task<List<UserChatDto>> GetGenreChatsAsync();
    Task<Guid> CreatePrivateChatAsync(Guid currentUserId, Guid otherUserId);
    Task<Guid?> GetPrivateChatIdAsync(Guid userId1, Guid userId2);

    Task<string> GetChatTypeAsync(Guid chatId);
}
