namespace DropBeatAPI.Core.DTOs.Beat;

public class FilterBeatsDto
{
    public string? SearchQuery { get; set; }
    public List<Guid> SelectedTags { get; set; } = new();
    public List<Guid> SelectedGenres { get; set; } = new();
    public List<Guid> SelectedMoods { get; set; } = new();
    public int MinPrice { get; set; } = 0;
    public int MaxPrice { get; set; } = 100000;
    public int MinBpm { get; set; } = 10;
    public int MaxBpm { get; set; } = 1000;
}