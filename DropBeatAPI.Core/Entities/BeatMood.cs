using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class BeatMood
    {
        public Guid BeatId { get; set; }
        public Beat Beat { get; set; } = null!;
        public Guid MoodId { get; set; }
        public Mood Mood { get; set; } = null!;
    }
}
