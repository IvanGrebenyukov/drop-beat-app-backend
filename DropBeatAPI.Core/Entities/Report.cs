using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid ReporterId { get; set; }
        public User Reporter { get; set; } = null!;
        public Guid ReportedUserId { get; set; }
        public User ReportedUser { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ReportStatus { Pending, Resolved }
}
