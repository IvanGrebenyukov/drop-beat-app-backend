using DropBeatAPI.Core.Entities;

namespace DropBeatAPI.Core.DTOs.Documents;

public class LicenseDocumentDto
{
    public Guid BeatId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public LicenseType LicenseType { get; set; }
    public DateTime CreatedAt { get; set; }
}