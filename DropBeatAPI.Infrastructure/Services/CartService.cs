using DropBeatAPI.Core.DTOs.Cart;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using DropBeatAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DropBeatAPI.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly BeatsDbContext _context;

    public CartService(BeatsDbContext context)
    {
        _context = context;
    }

    public async Task AddToCartAsync(Guid userId, Guid beatId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
        }

        if (cart.Items.Any(ci => ci.BeatId == beatId))
            throw new InvalidOperationException("Бит уже добавлен в корзину.");

        if (cart.Items.Count >= 5)
            throw new InvalidOperationException("Корзина не может содержать более 5 битов.");

        cart.Items.Add(new CartItem
        {
            CartId = cart.Id,
            BeatId = beatId,
            AddedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(Guid userId, Guid beatId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) throw new InvalidOperationException("Корзина не найдена.");

        var item = cart.Items.FirstOrDefault(ci => ci.BeatId == beatId);
        if (item == null) throw new InvalidOperationException("Бит не найден в корзине.");

        cart.Items.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<CartDto> GetCartAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Beat)
            .ThenInclude(b => b.Seller)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) 
            return new CartDto { CartId = Guid.NewGuid(), Items = new List<CartItemDto>() };
        
        
        var items = cart.Items.Select(ci => 
        {
            var beat = ci.Beat;
            string licenseDocument = "https://storage.yandexcloud.net/dropbeat/documents/free.pdf";
        
            if (beat.LicenseType == LicenseType.NonExclusive)
            {
                licenseDocument = "https://storage.yandexcloud.net/dropbeat/documents/non-exclusive.pdf";
            }
            else if (beat.LicenseType == LicenseType.Exclusive)
            {
                licenseDocument = "https://storage.yandexcloud.net/dropbeat/documents/exclusive.pdf";
            }

            return new CartItemDto
            {
                BeatId = beat.Id,
                Title = beat.Title,
                Price = beat.Price,
                BPM = beat.BPM,
                SellerName = beat.Seller.StageName ?? string.Empty,
                SellerId = beat.SellerId,
                CoverUrl = beat.CoverUrl,
                AudioKeyDemo = beat.AudioKey,
                LicenseFileUrl = licenseDocument
            };
        }).ToList();

        return new CartDto { CartId = cart.Id, Items = items };
    }
}
