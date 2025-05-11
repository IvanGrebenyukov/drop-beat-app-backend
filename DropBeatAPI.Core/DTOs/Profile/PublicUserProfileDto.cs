using DropBeatAPI.Core.DTOs.Genres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Profile
{
    public class PublicUserProfileDto
    {
        public string? StageName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public int? Age { get; set; }
        public string? Bio { get; set; }
        public bool IsSeller { get; set; }
        public List<UserSocialLinkDto> SocialLinks { get; set; } = new();
        public List<GenreDto> FavoriteGenres { get; set; } = new();
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}
