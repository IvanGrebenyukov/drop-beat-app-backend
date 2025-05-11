using DropBeatAPI.Core.DTOs.FavoriteBeat;
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
    public class BeatLikeService : IBeatLikeService
    {
        private readonly BeatsDbContext _context;

        public BeatLikeService (BeatsDbContext context)
        {
            _context = context;
        }

        public async Task AddToFavoritesAsync(Guid userId, Guid beatId)
        {
            var exists = await _context.BeatLikes.AnyAsync(bl => bl.UserId == userId 
            && bl.BeatId == beatId);
            if (exists)
            {
                return;
            }

            var like = new BeatLike
            {
                UserId = userId,
                BeatId = beatId,
                LikedAt = DateTime.UtcNow
            };

            _context.BeatLikes.Add(like);
            await _context.SaveChangesAsync();

        }

        public async Task RemoveFromFavoritesAsync(Guid userId, Guid beatId)
        {
            var like = await _context.BeatLikes.FirstOrDefaultAsync(bl => bl.UserId == userId
            && bl.BeatId == beatId);
            if (like == null) 
            { 
                return;
            }

            _context.BeatLikes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FavoriteBeatDto>> GetFavoriteBeatsAsync(Guid userId)
        {
            return await _context.BeatLikes
                .Where(bl => bl.UserId == userId)
                .Include(bl => bl.Beat).ThenInclude(b => b.Seller)
                .Select(bl => new FavoriteBeatDto
                {
                    BeatId = bl.Beat.Id,
                    SellerId = bl.Beat.SellerId,
                    StageName = bl.Beat.Seller.StageName ?? "",
                    Title = bl.Beat.Title,
                    Price = bl.Beat.Price,
                    LicenseType = bl.Beat.LicenseType.ToString(),
                    AudioKey = bl.Beat.AudioKey,
                    CoverUrl = bl.Beat.CoverUrl,
                    BPM = bl.Beat.BPM,
                    IsAvailable = bl.Beat.IsAvailable,
                }).ToListAsync();

        }

        public async Task<bool> CheckIfFavoriteAsync(Guid userId, Guid beatId)
        {
            return await _context.BeatLikes
                .AnyAsync(bl => bl.UserId == userId && bl.BeatId == beatId);
        }
    }
}
