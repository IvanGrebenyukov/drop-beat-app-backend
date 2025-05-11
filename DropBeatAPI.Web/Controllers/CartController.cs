using DropBeatAPI.Core.DTOs.Cart;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly UserManager<User> _userManager;

    public CartController(ICartService cartService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = GetUserId();
        await _cartService.AddToCartAsync(userId, dto.BeatId);
        return Ok(new { message = "Бит добавлен в корзину." });
    }

    [HttpDelete("remove/{beatId}")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(Guid beatId)
    {
        var userId = GetUserId();
        await _cartService.RemoveFromCartAsync(userId, beatId);
        return Ok(new { message = "Бит удален из корзины." });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    private Guid GetUserId()
    {
        return Guid.Parse(_userManager.GetUserId(User));
    }
}
