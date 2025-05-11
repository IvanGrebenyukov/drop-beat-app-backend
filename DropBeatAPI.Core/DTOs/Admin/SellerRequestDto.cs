using DropBeatAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Admin
{
    public class SellerRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public SellerRequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string? AdminComment { get; set; }
    }
}
