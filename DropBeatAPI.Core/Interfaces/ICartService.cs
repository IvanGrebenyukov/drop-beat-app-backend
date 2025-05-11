using DropBeatAPI.Core.DTOs.Cart;

namespace DropBeatAPI.Core.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(Guid userId, Guid beatId);
    Task RemoveFromCartAsync(Guid userId, Guid beatId);
    Task<CartDto> GetCartAsync(Guid userId);
}