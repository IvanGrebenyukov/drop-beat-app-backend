namespace DropBeatAPI.Core.Entities;

public class UserChat
{
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
