using DropBeatAPI.Core.DTOs.Profile;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(Guid userId);
        Task<bool> UpdateProfileAsync(Guid userId, UserProfileUpdateDto profileDto, IFormFile? avatarFile);
        Task<bool> UpdateSocialLinksAsync(Guid userId, List<UserSocialLinkDto> socialLinks);
        Task<PublicUserProfileDto?> GetPublicProfileAsync(Guid userId);
        Task<SellerRequestStatusDto> GetSellerRequestStatusAsync(Guid userId);
        Task<bool> SubmitSellerRequestAsync(Guid userId);
        Task<List<UserSocialLinkDto>> GetSocialLinksAsync(Guid userId);
        Task<List<SellerBeatDto>> GetBeatsBySellerIdAsync(Guid sellerId);
        Task<List<SellerBeatDto>> GetMyBeatsAsync(Guid userId);
    }
}
