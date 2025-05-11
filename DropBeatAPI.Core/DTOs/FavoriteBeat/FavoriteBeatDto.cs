using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.FavoriteBeat
{
    public class FavoriteBeatDto
    {
        public Guid BeatId { get; set; }
        public Guid SellerId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string LicenseType { get; set; } = string.Empty;
        public string AudioKey { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public int BPM { get; set; }
        public bool IsAvailable { get; set; }
    }
}
