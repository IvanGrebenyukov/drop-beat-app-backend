using DropBeatAPI.Core.DTOs.LogIn;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly BeatsDbContext _dbContext;

        public AuthService(
            UserManager<User> userManager,
            IConfiguration config,
            IEmailService emailService,
            BeatsDbContext dbContext)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _dbContext = dbContext;

        }

        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email.Split('@')[0],
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Join(", ", result.Errors));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var code = GenerateRandomCode();
            await SaveConfirmationCode(user.Id, code);

            // Отправляем код
            await _emailService.SendConfirmationEmail(user.Email, code);

            // Не генерируем JWT тут
            return new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                RequiresEmailConfirmation = true
            };
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Недействительные учетные данные");

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    UserId = user.Id.ToString(),
                    RequiresEmailConfirmation = true,
                    Message = "Требуется подтверждение email"
                };
            }

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<AuthResponseDto> ConfirmEmail(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null) throw new ApplicationException("Пользователь не найден");

            var codeEntity = await _dbContext.EmailConfirmationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == dto.Code);

            if (codeEntity == null || codeEntity.ExpiresAt < DateTime.UtcNow)
                throw new ApplicationException("Неверный или просроченный код");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            _dbContext.EmailConfirmationCodes.Remove(codeEntity);
            await _dbContext.SaveChangesAsync();

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        private string GenerateRandomCode(int length = 6)
        {
            var random = new Random();
            return new string(Enumerable.Repeat("0123456789", length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task SaveConfirmationCode(Guid userId, string code)
        {
            _dbContext.EmailConfirmationCodes.Add(new EmailConfirmationCode
            {
                UserId = userId,
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task ResendConfirmationCode(ResendEmailConfirmationDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null) throw new ApplicationException("Пользователь не найден");
            if (user.EmailConfirmed) throw new ApplicationException("Email уже подтвержден");

            var newCode = GenerateRandomCode();
            await SaveConfirmationCode(user.Id, newCode);

            await _emailService.SendConfirmationEmail(user.Email, newCode);
        }

        private async Task<AuthResponseDto> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["JwtSettings:ExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var userRole = roles.FirstOrDefault();
            decimal balance = 0;
            if (userRole == "Seller")
            {
                balance = user.Balance;
            }



            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                UserId = user.Id.ToString(),
                Role = userRole,
                Balance = balance 

            };
        }

        public Task<AuthResponseDto> LoginWithGoogle(GoogleLoginDto googleLoginDto)
        {
            throw new NotImplementedException();
        }


    }
}
