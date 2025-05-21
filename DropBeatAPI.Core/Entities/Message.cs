namespace DropBeatAPI.Core.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public Guid SenderId { get; set; }
    public User Sender { get; set; }

    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}