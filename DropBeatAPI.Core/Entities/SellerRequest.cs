using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class SellerRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public SellerRequestStatus Status { get; set; }
        public string? AdminComment { get; set; }
    }
}
