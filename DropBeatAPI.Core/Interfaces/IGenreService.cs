using DropBeatAPI.Core.DTOs.Genres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllGenres();
        Task<List<MoodsDto>> GetAllMoods();
        Task CreateDefaultMoods();
        Task CreateDefaultGenres();
        Task SelectGenres(string userId, SelectGenresDto dto);
    }
}
