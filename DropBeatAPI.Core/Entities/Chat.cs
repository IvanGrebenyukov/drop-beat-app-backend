namespace DropBeatAPI.Core.Entities;

public class Chat
{
    public Guid Id { get; set; }

    // Тип чата: Личный или Жанровый
    public ChatType Type { get; set; }

    // Только для ChatType = Genre
    public Guid? GenreId { get; set; }
    public Genre? Genre { get; set; }

    // Только для ChatType = Private
    public ICollection<UserChat> Participants { get; set; } = new List<UserChat>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public enum ChatType
{
    Private = 0,
    Genre = 1
}