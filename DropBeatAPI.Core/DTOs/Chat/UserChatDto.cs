using DropBeatAPI.Core.Entities;

namespace DropBeatAPI.Core.DTOs.Chat;

public class UserChatDto
{
    public Guid ChatId { get; set; }
    public ChatType Type { get; set; }
    public string? GenreName { get; set; }
    public Guid? GenreId { get; set; }
    public List<string> Participants { get; set; } = new();
    public List<Guid> ParticipantsId { get; set; } = new();
    public string? LastMessage { get; set; }
    public DateTime? LastSentAt { get; set; }
}