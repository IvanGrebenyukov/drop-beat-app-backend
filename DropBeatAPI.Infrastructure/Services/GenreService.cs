using DropBeatAPI.Core.DTOs.Genres;
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
    public class GenreService : IGenreService
    {
        private readonly BeatsDbContext _dbContext;

        public GenreService(BeatsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateDefaultGenres()
        {
            if (await _dbContext.Genres.AnyAsync()) return; // Чтобы не дублировать

            var genres = new List<Genre>
            {
                new() { Name = "Hip-Hop", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Trap", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Pop", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Lo-Fi", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Drill", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Boom Bap", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "RnB", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "House", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Techno", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Phonk", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Jazz", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
                new() { Name = "Country", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/genres/genre_hip_hop.png" },
            };

            await _dbContext.Genres.AddRangeAsync(genres);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateDefaultMoods()
        {
            if (await _dbContext.Moods.AnyAsync()) return; // Чтобы не дублировать

            var moods = new List<Mood>
            {
                new() { Name = "Bouncy", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Dark", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Energetic", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Soulful", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Sad", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Relaxed", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Angry", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Mellow", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Epic", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Crazy", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Dirty", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Romantic", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Dramatic", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
                new() { Name = "Lazy", IconUrl = "https://storage.yandexcloud.net/dropbeat/icons/moods/sad_mood.png" },
            };

            await _dbContext.Moods.AddRangeAsync(moods);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GenreDto>> GetAllGenres()
        {
            return await _dbContext.Genres
                .Select(g => new GenreDto { Id = g.Id, Name = g.Name, IconUrl = g.IconUrl })
                .ToListAsync();
        }

        public async Task<List<MoodsDto>> GetAllMoods()
        {
            return await _dbContext.Moods
                .Select(m => new MoodsDto { Id = m.Id, Name = m.Name, IconUrl = m.IconUrl })
                .ToListAsync();
        }


        public async Task SelectGenres(string userId, SelectGenresDto dto)
        {
            if (dto.GenreIds == null || !dto.GenreIds.Any())
            {
                throw new ApplicationException("Выберите хотя бы один жанр.");
            }

            var userGuid = Guid.Parse(userId);
            var user = await _dbContext.Users
                .Include(u => u.UserGenres)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                throw new ApplicationException("Пользователь не найден.");
            }

            var existingGenreIds = await _dbContext.Genres
                .Where(g => dto.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            if (existingGenreIds.Count != dto.GenreIds.Count)
            {
                throw new ApplicationException("Один или несколько жанров не существуют");
            }

            var existingGenres = user.UserGenres.ToList();
            _dbContext.UserGenres.RemoveRange(existingGenres); // Удаляем старые жанры

            user.UserGenres = dto.GenreIds
                .Select(genreId => new UserGenre { UserId = user.Id, GenreId = genreId })
                .ToList();

            await _dbContext.SaveChangesAsync();
        }
        
        
    }
}
