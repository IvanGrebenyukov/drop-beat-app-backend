using DropBeatAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Profile
{
    public class UserSocialLinkDto
    {
        public SocialPlatform Platform { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
