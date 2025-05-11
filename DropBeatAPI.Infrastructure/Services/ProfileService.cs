using DropBeatAPI.Core.DTOs.Genres;
using DropBeatAPI.Core.DTOs.Profile;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly BeatsDbContext _context;
        private readonly YandexStorageService _storageService;

        public ProfileService(UserManager<User> userManager, BeatsDbContext context, YandexStorageService storageService)
        {
            _userManager = userManager;
            _context = context;
            _storageService = storageService;
        }

        public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
        {
           var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                StageName = user.StageName,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Age = user.Age,
                Address = user.Address,
                Bio = user.Bio,
            };
        }

        public async Task<PublicUserProfileDto?> GetPublicProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.SocialLinks)
                .Include(u => u.UserGenres)
                    .ThenInclude(ug => ug.Genre)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null) return null;

            return new PublicUserProfileDto
            {
                StageName = user.StageName,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                Age = user.Age,
                Bio = user.Bio,
                IsSeller = user.IsSeller,
                SocialLinks = user.SocialLinks.Select(sl => new UserSocialLinkDto
                {
                    Platform = sl.Platform,
                    Url = sl.Url,
                }).ToList(),
                FavoriteGenres = user.UserGenres.Select(ug => new GenreDto
                {
                    Id = ug.Genre.Id,
                    Name = ug.Genre.Name,
                    IconUrl = ug.Genre.IconUrl,
                }).ToList(),
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
            };
        }

        public async Task<SellerRequestStatusDto> GetSellerRequestStatusAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new SellerRequestStatusDto
                {
                    Status = SellerRequestStatus.None,
                    AdminComment = null
                };
            }

            // Получаем последнюю заявку пользователя
            var lastRequest = await _context.SellerRequests
                .Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.RequestDate) // Берем последнюю по дате
                .FirstOrDefaultAsync();

            return new SellerRequestStatusDto
            {
                Status = user.SellerRequestStatus,
                AdminComment = lastRequest?.AdminComment // Если комментарий есть — добавляем
            };
        }


        public async Task<bool> SubmitSellerRequestAsync(Guid userId)
        {
            var user = await _context.Users.Include(u => u.SocialLinks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.SellerRequestStatus == SellerRequestStatus.Pending || user.SellerRequestStatus == SellerRequestStatus.Approved)
                return false;

            // Подсчитываем количество отказов
            int rejectionCount = await _context.SellerRequests
                .Where(sr => sr.UserId == userId && sr.Status == SellerRequestStatus.Rejected)
                .CountAsync();

            if (rejectionCount >= 3)
                return false; // Блокируем возможность подачи заявки

            user.SellerRequestStatus = SellerRequestStatus.Pending;

            var sellerRequest = new SellerRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = SellerRequestStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            _context.SellerRequests.Add(sellerRequest);

            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> UpdateProfileAsync(Guid userId, UserProfileUpdateDto profileDto, IFormFile? avatarFile)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user == null) return false;

            if(avatarFile != null)
            {
                using var stream = avatarFile.OpenReadStream();
                string contentType = avatarFile.ContentType;
                string filename = avatarFile.FileName;

                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var oldFileKey = user.AvatarUrl.Split('/').Last();
                    await _storageService.DeleteFileAsync($"avatars/{oldFileKey}");
                }

                var avatarUrl = await _storageService.UploadFileAsync(stream, filename, contentType, "avatars");
                user.AvatarUrl = avatarUrl;
            }

            user.StageName = profileDto.StageName;
            user.FirstName = profileDto.FirstName;
            user.LastName = profileDto.LastName;
            user.MiddleName = profileDto.MiddleName;
            user.Age = profileDto.Age;
            user.Address = profileDto.Address;
            user.Bio = profileDto.Bio;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<UserSocialLinkDto>> GetSocialLinksAsync(Guid userId)
        {
            var socialLinks = await _context.UserSocialLinks
                .Where(link => link.UserId == userId)
                .Select(link => new UserSocialLinkDto
                {
                    Platform = link.Platform,
                    Url = link.Url
                })
                .ToListAsync();

            return socialLinks;
        }

        public async Task<bool> UpdateSocialLinksAsync(Guid userId, List<UserSocialLinkDto> socialLinks)
        {
            var user = await _context.Users.Include(u => u.SocialLinks)
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            foreach (var linkDto in socialLinks)
            {
                var existingLink = user.SocialLinks.FirstOrDefault(l => l.Platform == linkDto.Platform);
                if (existingLink != null)
                {
                    existingLink.Url = linkDto.Url;
                }
                else
                {
                    user.SocialLinks.Add(new UserSocialLink
                    {
                        UserId = userId,
                        Platform = linkDto.Platform,
                        Url = linkDto.Url
                    });
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<SellerBeatDto>> GetBeatsBySellerIdAsync(Guid sellerId)
        {
            var user = await _context.Users.FindAsync(sellerId);
            if (user == null) return new();

            return await _context.Beats
                .Where(b => b.SellerId == sellerId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new SellerBeatDto
                {
                    BeatId = b.Id,
                    SellerId = b.SellerId,
                    StageName = user.StageName ?? string.Empty,
                    Title = b.Title,
                    Price = b.Price,
                    LicenseType = b.LicenseType.ToString(),
                    AudioKey = b.AudioKey,
                    CoverUrl = b.CoverUrl,
                    BPM = b.BPM,
                    IsAvailable = b.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<List<SellerBeatDto>> GetMyBeatsAsync(Guid userId)
        {
            return await GetBeatsBySellerIdAsync(userId);
        }

    }
}
