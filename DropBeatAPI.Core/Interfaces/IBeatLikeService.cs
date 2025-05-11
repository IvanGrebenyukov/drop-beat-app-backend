using DropBeatAPI.Core.DTOs.FavoriteBeat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IBeatLikeService
    {
        Task AddToFavoritesAsync(Guid userId, Guid beatId);
        Task RemoveFromFavoritesAsync(Guid userId, Guid beatId);
        Task<List<FavoriteBeatDto>> GetFavoriteBeatsAsync(Guid userId);
        Task<bool> CheckIfFavoriteAsync(Guid userId, Guid beatId);
    }
}
