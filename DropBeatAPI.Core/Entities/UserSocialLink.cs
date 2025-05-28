using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Entities
{
    public class UserSocialLink
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public SocialPlatform Platform { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    public enum SocialPlatform { Vk, YouTube, Telegram, TikTok, SoundCloud }
}
