using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public ICollection<UserGenre> UserGenres { get; set; } = new List<UserGenre>();
        public ICollection<BeatGenre> BeatGenres { get; set; } = new List<BeatGenre>();
    }
}
