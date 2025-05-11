using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class CartItem
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public Guid BeatId { get; set; }
        public Beat Beat { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
