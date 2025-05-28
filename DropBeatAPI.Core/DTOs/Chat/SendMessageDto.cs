namespace DropBeatAPI.Core.DTOs.Chat;

public class SendMessageDto
{
    public Guid? ReceiverId { get; set; } // Только для личного чата
    public Guid? GenreId { get; set; } // Только для жанрового чата
    public string Text { get; set; } = string.Empty;
}