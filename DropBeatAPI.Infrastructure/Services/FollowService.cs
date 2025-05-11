using DropBeatAPI.Core.DTOs.Follow;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Services
{
    public class FollowService : IFollowService
    {
        private readonly BeatsDbContext _context;

        public FollowService (BeatsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FollowUserAsync(Guid followerId, Guid followingId)
        {
            if(followerId == followingId) return false;

            var exists = await _context.Follows.AnyAsync(f => f.FollowerId == followerId
            && f.FollowingId == followingId);

            if(exists) return false;

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow,
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
            return true;
        } 

        public async Task<bool> UnfollowUserAsync (Guid followerId, Guid followingId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId 
                && f.FollowingId == followingId);

            if(follow == null) return false;

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<FollowDto>> GetFollowersAsync(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Select(f => new FollowDto
                {
                    UserId = f.Follower.Id,
                    StageName = f.Follower.StageName,
                    AvatarUrl = f.Follower.AvatarUrl,
                    IsSeller = f.Follower.IsSeller,
                })
                .ToListAsync();
        }

        public async Task<List<FollowDto>> GetFollowingAsync(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId  == userId)
                .Select(f => new FollowDto
                {
                    UserId = f.Following.Id,
                    StageName = f.Following.StageName,
                    AvatarUrl = f.Following.AvatarUrl,
                    IsSeller = f.Following.IsSeller,
                })
                .ToListAsync();
        }

        public async Task<List<FollowDto>> SearchFollowersAsync(Guid userId, string stageName)
        {
            return await _context.Follows
                .Where(f => f.FollowingId == userId && f.Follower.StageName!.ToLower().Contains(stageName.ToLower()))
                .Select(f => new FollowDto
                {
                    UserId = f.Follower.Id,
                    StageName = f.Follower.StageName,
                    AvatarUrl = f.Follower.AvatarUrl
                })
                .ToListAsync();
        }

        public async Task<List<FollowDto>> SearchFollowingAsync(Guid userId, string stageName)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId && f.Following.StageName!.ToLower().Contains(stageName.ToLower()))
                .Select(f => new FollowDto
                {
                    UserId = f.Following.Id,
                    StageName = f.Following.StageName,
                    AvatarUrl = f.Following.AvatarUrl
                })
                .ToListAsync();
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }
    }
}
