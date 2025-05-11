using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Profile
{
    public class UserProfileDto
    {
        public string? StageName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
}
