using DropBeatAPI.Core.DTOs.Follow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IFollowService
    {
        Task<bool> FollowUserAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);

        Task<List<FollowDto>> GetFollowersAsync(Guid userId);
        Task<List<FollowDto>> GetFollowingAsync(Guid userId);

        Task<List<FollowDto>> SearchFollowersAsync(Guid userId, string stageName);
        Task<List<FollowDto>> SearchFollowingAsync(Guid userId, string stageName);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    }

}
