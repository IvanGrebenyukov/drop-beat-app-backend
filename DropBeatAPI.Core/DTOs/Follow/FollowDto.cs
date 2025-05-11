using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Follow
{
    public class FollowDto
    {
        public Guid UserId { get; set; }
        public string? StageName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsSeller { get; set; }
    }


}
