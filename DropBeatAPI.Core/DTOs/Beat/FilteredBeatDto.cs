namespace DropBeatAPI.Core.DTOs.Beat;

public class FilteredBeatDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int BPM { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string CoverUrl { get; set; } = string.Empty;
    public string AudioKeyDemo { get; set; } = string.Empty;
}