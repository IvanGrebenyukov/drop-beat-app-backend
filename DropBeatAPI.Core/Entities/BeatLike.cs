using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class BeatLike
    {
        // Составной первичный ключ
        public Guid UserId { get; set; }
        public Guid BeatId { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Beat Beat { get; set; } = null!;
    }
}
