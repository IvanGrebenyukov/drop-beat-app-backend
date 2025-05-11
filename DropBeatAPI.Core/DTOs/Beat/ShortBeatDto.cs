using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Beat
{
    public class ShortBeatDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int BPM { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public Guid SellerId { get; set; }
        public string? CoverUrl { get; set; }
        public string AudioKeyDemo { get; set; } = string.Empty;
    }
}
