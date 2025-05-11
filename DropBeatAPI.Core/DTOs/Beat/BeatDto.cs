using DropBeatAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Beat
{
    public class BeatDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int BPM { get; set; }
        public LicenseType LicenseType { get; set; }
        public bool IsAvailable { get; set; }
        public string AudioKeyDemo { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string LicenseDocument { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;

        public List<string> Genres { get; set; } = new();
        public List<string> Moods { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

}
