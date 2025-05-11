using DropBeatAPI.Core.DTOs.Admin;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly BeatsDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminService(BeatsDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<SellerRequestDto>> GetSellerRequestsAsync()
        {
            var latestRequests = await _context.SellerRequests
                .Where(sr => sr.Status == SellerRequestStatus.Pending) // Только необработанные заявки
                .Include(sr => sr.User) // Загружаем пользователя до группировки
                .GroupBy(sr => sr.UserId) // Группируем по пользователю
                .Select(g => g.OrderByDescending(sr => sr.RequestDate).FirstOrDefault()) // Берем последнюю заявку
                .ToListAsync();

            return latestRequests
                .Where(sr => sr != null) // Фильтруем null значения
                .Select(sr => new SellerRequestDto
                {
                    Id = sr!.Id,
                    UserId = sr.UserId,
                    StageName = sr.User.StageName,
                    AvatarUrl = sr.User.AvatarUrl,
                    RequestDate = sr.RequestDate,
                    Status = sr.Status,
                    AdminComment = sr.AdminComment
                })
                .ToList();
        }




        public async Task<bool> ApproveSellerRequestAsync(Guid userId, string adminComment)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.SellerRequestStatus != SellerRequestStatus.Pending) return false;

            // Получаем текущие роли пользователя
            var roles = await _userManager.GetRolesAsync(user);

            // Удаляем все роли (если у пользователя их несколько, например User и Seller)
            await _userManager.RemoveFromRolesAsync(user, roles);

            // Назначаем только роль "Seller"
            await _userManager.AddToRoleAsync(user, "Seller");

            // Обновляем статус пользователя
            user.IsSeller = true;
            user.SellerRequestStatus = SellerRequestStatus.Approved;

            // Добавляем запись в SellerRequests
            var sellerRequest = new SellerRequest
            {
                UserId = userId,
                Status = SellerRequestStatus.Approved,
                AdminComment = adminComment
            };
            _context.SellerRequests.Add(sellerRequest);

            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> RejectSellerRequestAsync(Guid userId, string adminComment)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.SellerRequestStatus != SellerRequestStatus.Pending) return false;

            user.SellerRequestStatus = SellerRequestStatus.Rejected;

            var sellerRequest = new SellerRequest
            {
                UserId = userId,
                Status = SellerRequestStatus.Rejected,
                AdminComment = adminComment
            };
            _context.SellerRequests.Add(sellerRequest);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
