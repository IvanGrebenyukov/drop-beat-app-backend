using DropBeatAPI.Core.Entities;

namespace DropBeatAPI.Core.DTOs.Chat;

public class ChatDto
{
    public Guid ChatId { get; set; }
    public ChatType Type { get; set; }
    public string? GenreName { get; set; } // Для жанрового чата
    public List<MessageDto> Messages { get; set; } = new();
}