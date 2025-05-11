using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class Beat
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public User Seller { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int BPM { get; set; }
        public LicenseType LicenseType { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string AudioKey { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public ICollection<BeatGenre> Genres { get; set; } = new List<BeatGenre>();
        public ICollection<BeatMood> Moods { get; set; } = new List<BeatMood>();
        public ICollection<BeatTag> Tags { get; set; } = new List<BeatTag>();
        public ICollection<BeatLike> Likes { get; set; } = new List<BeatLike>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }

    public enum LicenseType { Free, NonExclusive, Exclusive }
}
