using DropBeatAPI.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Beat
{
    public class CreateBeatDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int BPM { get; set; }
        public LicenseType LicenseType { get; set; }
        public List<Guid> GenreIds { get; set; } = new();
        public List<Guid> MoodIds { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public IFormFile AudioFile { get; set; } = null!;
        public IFormFile? CoverFile { get; set; }
    }

}
