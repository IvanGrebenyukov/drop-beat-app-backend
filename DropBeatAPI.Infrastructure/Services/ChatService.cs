using AutoMapper;
using DropBeatAPI.Core.DTOs.Chat;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using DropBeatAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DropBeatAPI.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly BeatsDbContext _dbContext;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatService(BeatsDbContext dbContext, IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto)
    {
        Chat chat;

        if (dto.GenreId.HasValue)
        {
            chat = await _dbContext.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Type == ChatType.Genre && c.GenreId == dto.GenreId)
                ?? throw new Exception("Жанровый чат не найден");
        }
        else if (dto.ReceiverId.HasValue)
        {
            var existingChat = await _dbContext.UserChats
                .Where(uc => uc.UserId == senderId || uc.UserId == dto.ReceiverId)
                .GroupBy(uc => uc.ChatId)
                .Where(g => g.Count() == 2)
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (existingChat != Guid.Empty)
            {
                chat = await _dbContext.Chats.FindAsync(existingChat)
                    ?? throw new Exception("Чат не найден");
            }
            else
            {
                chat = new Chat { Type = ChatType.Private };
                _dbContext.Chats.Add(chat);
                await _dbContext.SaveChangesAsync();

                _dbContext.UserChats.AddRange(
                    new UserChat { ChatId = chat.Id, UserId = senderId },
                    new UserChat { ChatId = chat.Id, UserId = dto.ReceiverId.Value }
                );
                await _dbContext.SaveChangesAsync();
            }
        }
        else throw new Exception("Нужно указать GenreId или ReceiverId");

        var message = new Message
        {
            ChatId = chat.Id,
            SenderId = senderId,
            Text = dto.Text,
            SentAt = DateTime.UtcNow
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        var sender = await _dbContext.Users.FindAsync(senderId);
        return new MessageDto
        {
            Id = message.Id,
            ChatId = chat.Id,
            SenderId = senderId,
            SenderName = sender?.StageName ?? "Unknown",
            Text = message.Text,
            SentAt = message.SentAt
        };
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid chatId)
    {
        return await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                SenderName = m.Sender.StageName,
                Text = m.Text,
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    public async Task<List<UserChatDto>> GetPrivateChatsAsync(Guid userId)
    {
        var chatIds = await _dbContext.UserChats
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.ChatId)
            .ToListAsync();

        var privateChats = await _dbContext.Chats
            .Where(c => chatIds.Contains(c.Id) && c.Type == ChatType.Private)
            .Include(c => c.Participants).ThenInclude(pc => pc.User)
            .Include(c => c.Messages)
            .ToListAsync();

        return privateChats.Select(c => new UserChatDto
        {
            ChatId = c.Id,
            Type = c.Type,
            GenreName = null,
            Participants = c.Participants.Select(p => p.User.StageName).ToList(),
            ParticipantsId = c.Participants.Select(p => p.UserId).ToList(),
            LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Text,
            LastSentAt = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt
        }).ToList();
    }
    
    public async Task<List<UserChatDto>> GetGenreChatsAsync()
    {
        var genreChats = await _dbContext.Chats
            .Where(c => c.Type == ChatType.Genre)
            .Include(c => c.Genre)
            .Include(c => c.Messages)
            .ToListAsync();

        return genreChats.Select(c => new UserChatDto
        {
            ChatId = c.Id,
            Type = c.Type,
            GenreName = c.Genre?.Name,
            GenreId = c.Genre?.Id,
            Participants = new List<string>(), // жанровый чат — без списка участников
            LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Text,
            LastSentAt = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt
        }).ToList();
    }
    
    public async Task<Guid> CreatePrivateChatAsync(Guid currentUserId, Guid otherUserId)
    {
        if (currentUserId == otherUserId)
            throw new Exception("Нельзя создать чат с самим собой.");

        // Проверка, существует ли уже чат между двумя пользователями
        var existingChatId = await GetPrivateChatIdAsync(currentUserId, otherUserId);
        if (existingChatId != null)
            return existingChatId.Value;

        // Создаем новый чат
        var chat = new Chat { Type = ChatType.Private };
        _dbContext.Chats.Add(chat);
        await _dbContext.SaveChangesAsync();

        _dbContext.UserChats.AddRange(
            new UserChat { ChatId = chat.Id, UserId = currentUserId },
            new UserChat { ChatId = chat.Id, UserId = otherUserId }
        );
        await _dbContext.SaveChangesAsync();

        return chat.Id;
    }
    
    public async Task<Guid?> GetPrivateChatIdAsync(Guid userId1, Guid userId2)
    {
        var chatId = await _dbContext.UserChats
            .Where(uc => uc.UserId == userId1 || uc.UserId == userId2)
            .GroupBy(uc => uc.ChatId)
            .Where(g => g.Count() == 2)
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        return chatId == Guid.Empty ? null : chatId;
    }
    
    public async Task<string> GetChatTypeAsync(Guid chatId)
    {
        var chat = await _dbContext.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Чат не найден");

        return chat.Type.ToString(); // "Private" или "Genre"
    }


}

