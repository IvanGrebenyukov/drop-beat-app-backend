using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class Mood
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public ICollection<BeatMood> BeatMoods { get; set; } = new List<BeatMood>();
    }
}
