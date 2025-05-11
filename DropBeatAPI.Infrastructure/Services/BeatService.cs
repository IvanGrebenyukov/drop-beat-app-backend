using DropBeatAPI.Core.DTOs.Beat;
using DropBeatAPI.Core.DTOs.Genres;
using DropBeatAPI.Core.DTOs.Tag;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using DropBeatAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropBeatAPI.Core.DTOs.Documents;

namespace DropBeatAPI.Infrastructure.Services
{
    public class BeatService : IBeatService
    {
        private readonly BeatsDbContext _context;
        private readonly YandexStorageService _storageService;
        private readonly WatermarkService _watermarkService;
        private LicenseService _licenseService;

        public BeatService(BeatsDbContext context, YandexStorageService storageService, WatermarkService watermarkService, LicenseService licenseService)
        {
            _context = context;
            _storageService = storageService;
            _watermarkService = watermarkService;
            _licenseService = licenseService;
        }

        public async Task<Guid> AddBeatAsync(CreateBeatDto dto, Guid sellerId)
        {
            if (dto.GenreIds.Count < 1 || dto.GenreIds.Count > 3)
                throw new ArgumentException("Выберите от 1 до 3 жанров.");

            if (dto.MoodIds.Count < 1 || dto.MoodIds.Count > 3)
                throw new ArgumentException("Выберите от 1 до 3 настроений.");

            if (dto.Tags.Count < 1 || dto.Tags.Count > 5)
                throw new ArgumentException("Выберите от 1 до 5 тегов.");

            if (dto.LicenseType == LicenseType.Free)
                dto.Price = 0;
            else if (dto.Price < 500 || dto.Price > 20000 || dto.Price % 500 != 0)
                throw new ArgumentException("Цена должна быть от 500 до 20000 с шагом 500.");

            var beatId = Guid.NewGuid();
            var licenseDto = new LicenseDocumentDto
            {
                BeatId = beatId,
                Title = dto.Title,
                SellerName = (await _context.Users.FindAsync(sellerId))?.StageName ?? "Неизвестно",
                LicenseType = dto.LicenseType,
                CreatedAt = DateTime.UtcNow
            };
            
            // Генерация PDF лицензии
            var licenseStream = await _licenseService.GenerateLicensePdfAsync(licenseDto);

            // Загрузка лицензии в Object Storage
            var licenseKey = await _storageService.UploadFileAsync(
                licenseStream, 
                $"{beatId}.pdf", 
                "application/pdf", 
                "licenses"
            );
            
            var originalKey = await _storageService.UploadFileAsync(dto.AudioFile, $"{beatId}.mp3", "audio/mpeg", "audios/original");
            var demoStream = await _watermarkService.AddWatermarkAsync(dto.AudioFile);
            var demoKey = await _storageService.UploadFileAsync(demoStream, $"{beatId}.mp3", "audio/mpeg", "audios/demo");


            var coverKey = string.Empty;
            if (dto.CoverFile != null)
            {
                coverKey = await _storageService.UploadFileAsync(dto.CoverFile, $"{beatId}.jpg", "image/jpeg", "covers");
            }

            var beat = new Beat
            {
                Id = beatId,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                BPM = dto.BPM,
                LicenseType = dto.LicenseType,
                AudioKey = demoKey,
                CoverUrl = coverKey,
                LicenseUrl = licenseKey,
                SellerId = sellerId
            };

            _context.Beats.Add(beat);

            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            beat.Genres = genres.Select(g => new BeatGenre { BeatId = beatId, GenreId = g.Id }).ToList();

            var moods = await _context.Moods.Where(m => dto.MoodIds.Contains(m.Id)).ToListAsync();
            beat.Moods = moods.Select(m => new BeatMood { BeatId = beatId, MoodId = m.Id }).ToList();

            foreach (var tagName in dto.Tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                    _context.Tags.Add(tag);
                }
                beat.Tags.Add(new BeatTag { BeatId = beatId, TagId = tag.Id });
            }

            await _context.SaveChangesAsync();
            return beatId;
        }

        public async Task<List<ShortBeatDto>> GetAllBeatsAsync()
        {
            return await _context.Beats
                .Include(b => b.Seller)
                .Select(b => new ShortBeatDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    BPM = b.BPM,
                    SellerName = b.Seller.StageName,
                    SellerId = b.Seller.Id,
                    AudioKeyDemo = b.AudioKey,
                    CoverUrl = !string.IsNullOrEmpty(b.CoverUrl) ? b.CoverUrl : ""
                }).ToListAsync();
        }

        public async Task<ShortBeatDto?> GetShortBeatByIdAsync(Guid beatId)
        {
            var beat = await _context.Beats.Include(b => b.Seller).FirstOrDefaultAsync(b => b.Id == beatId);
            if (beat == null) return null;

            return new ShortBeatDto
            {
                Id = beat.Id,
                Title = beat.Title,
                Price = beat.Price,
                BPM = beat.BPM,
                SellerName = beat.Seller.StageName,
                SellerId = beat.Seller.Id,
                AudioKeyDemo = beat.AudioKey,
                CoverUrl = !string.IsNullOrEmpty(beat.CoverUrl) ? beat.CoverUrl : ""
            };
        }

        public async Task<BeatDto?> GetBeatByIdAsync(Guid beatId)
        {
            var beat = await _context.Beats
                .Include(b => b.Seller)
                .Include(b => b.Genres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Moods).ThenInclude(bm => bm.Mood)
                .Include(b => b.Tags).ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Id == beatId);

            if (beat == null) return null;

            string licenceDocument = "https://storage.yandexcloud.net/dropbeat/documents/free.pdf";
            if (beat.LicenseType == LicenseType.NonExclusive)
            {
                licenceDocument = "https://storage.yandexcloud.net/dropbeat/documents/non-exclusive.pdf";
            }
            else if (beat.LicenseType == LicenseType.Exclusive)
            {
                licenceDocument = "https://storage.yandexcloud.net/dropbeat/documents/exclusive.pdf";
            }

            return new BeatDto
            {
                Id = beat.Id,
                Title = beat.Title,
                Description = beat.Description,
                Price = beat.Price,
                BPM = beat.BPM,
                LicenseType = beat.LicenseType,
                IsAvailable = beat.IsAvailable,
                AudioKeyDemo = beat.AudioKey,
                CoverUrl = !string.IsNullOrEmpty(beat.CoverUrl) ? beat.CoverUrl : "",
                LicenseDocument = licenceDocument,
                CreatedAt = beat.CreatedAt,
                SellerId = beat.SellerId,
                SellerName = beat.Seller.StageName,
                Genres = beat.Genres.Select(g => g.Genre.Name).ToList(),
                Moods = beat.Moods.Select(m => m.Mood.Name).ToList(),
                Tags = beat.Tags.Select(t => t.Tag.Name).ToList()
            };
        }

        public async Task<bool> DeleteBeatAsync(Guid beatId, Guid sellerId)
        {
            var beat = await _context.Beats
                .Include(b => b.Genres)
                .Include(b => b.Moods)
                .Include(b => b.Tags)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Id == beatId && b.SellerId == sellerId);

            if (beat == null) return false;

            // Удаляем файлы из Object Storage
            try
            {
                // Удаляем demo версию (AudioKey)
                if (!string.IsNullOrEmpty(beat.AudioKey))
                {
                    var demoKey = ExtractObjectKeyFromUrl(beat.AudioKey);
                    await _storageService.DeleteFileAsync(demoKey);
                }

                // Удаляем original версию (паттерн: "audios/original/{beatId}.mp3")
                var originalKey = $"audios/original/{beatId}.mp3";
                await _storageService.DeleteFileAsync(originalKey);

                // Удаляем обложку (если есть)
                if (!string.IsNullOrEmpty(beat.CoverUrl))
                {
                    var coverKey = ExtractObjectKeyFromUrl(beat.CoverUrl);
                    await _storageService.DeleteFileAsync(coverKey);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но продолжаем удаление из БД
                Console.WriteLine($"Ошибка при удалении файлов: {ex.Message}");
            }

            // Удаляем запись из базы данных
            _context.Beats.Remove(beat);
            return await _context.SaveChangesAsync() > 0;
        }

        // Вспомогательный метод для извлечения ключа объекта из URL
        private string ExtractObjectKeyFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                var uri = new Uri(url);
                // Получаем путь после домена (например: "dropbeat/audios/demo/12345.mp3")
                var fullPath = uri.AbsolutePath.TrimStart('/');

                // Удаляем имя бакета из начала пути
                if (fullPath.StartsWith("dropbeat/"))
                {
                    return fullPath.Substring("dropbeat/".Length);
                }

                return fullPath;
            }
            catch
            {
                return url; // Если URL не валиден, возвращаем как есть
            }
        }

        // Получение случайных тегов (до 25)
        public async Task<List<TagDto>> GetRandomTagsAsync()
        {
            var totalTagsCount = await _context.Tags.CountAsync();
            var takeCount = Math.Min(25, totalTagsCount);

            return await _context.Tags
                .OrderBy(t => Guid.NewGuid()) // Случайная сортировка
                .Take(takeCount)
                .Select(t => new TagDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        // Поиск тегов по строке
        public async Task<List<TagDto>> SearchTagsAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return new List<TagDto>();

            return await _context.Tags
                .Where(t => t.Name.Contains(searchString))
                .Select(t => new TagDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }
        
        public async Task<List<FilteredBeatDto>> GetFilteredBeatsAsync(FilterBeatsDto filterDto)
        {
            var query = _context.Beats
                .Include(b => b.Seller)
                .Include(b => b.Tags)
                .Include(b => b.Genres)
                .Include(b => b.Moods)
                .AsQueryable();

            // Фильтр по названию (если строка не пустая)
            if (!string.IsNullOrWhiteSpace(filterDto.SearchQuery))
            {
                query = query.Where(b => b.Title.Contains(filterDto.SearchQuery));
            }

            // Фильтр по тегам (если есть выбранные теги)
            if (filterDto.SelectedTags != null && filterDto.SelectedTags.Any())
            {
                query = query.Where(b => b.Tags.Any(t => filterDto.SelectedTags.Contains(t.TagId)));
            }

            // Фильтр по жанрам (если есть выбранные жанры)
            if (filterDto.SelectedGenres != null && filterDto.SelectedGenres.Any())
            {
                query = query.Where(b => b.Genres.Any(g => filterDto.SelectedGenres.Contains(g.GenreId)));
            }

            // Фильтр по настроению (если есть выбранные настроения)
            if (filterDto.SelectedMoods != null && filterDto.SelectedMoods.Any())
            {
                query = query.Where(b => b.Moods.Any(m => filterDto.SelectedMoods.Contains(m.MoodId)));
            }

            // Фильтр по цене (если диапазон отличается от стандартного)
            if (filterDto.MinPrice > 0 || filterDto.MaxPrice < 100000)
            {
                query = query.Where(b => b.Price >= filterDto.MinPrice && b.Price <= filterDto.MaxPrice);
            }

            // Фильтр по BPM (если диапазон отличается от стандартного)
            if (filterDto.MinBpm > 10 || filterDto.MaxBpm < 1000)
            {
                query = query.Where(b => b.BPM >= filterDto.MinBpm && b.BPM <= filterDto.MaxBpm);
            }

            var filteredBeats = await query
                .Select(b => new FilteredBeatDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    BPM = b.BPM,
                    SellerName = b.Seller.StageName ?? string.Empty,
                    SellerId = b.SellerId,
                    CoverUrl = b.CoverUrl,
                    AudioKeyDemo = b.AudioKey
                })
                .ToListAsync();

            return filteredBeats;
        }

        //public async Task<bool> DeleteBeatAsync(Guid beatId, Guid sellerId)
        //{
        //    var beat = await _context.Beats
        //        .Include(b => b.Genres)
        //        .Include(b => b.Moods)
        //        .Include(b => b.Tags)
        //        .Include(b => b.Likes)
        //        .FirstOrDefaultAsync(b => b.Id == beatId && b.SellerId == sellerId);

        //    if (beat == null) return false;

        //    _context.Beats.Remove(beat);
        //    return await _context.SaveChangesAsync() > 0;
        //}

    }
}
