using DropBeatAPI.Core.DTOs.FavoriteBeat;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeatLikesController : ControllerBase
    {
        private readonly IBeatLikeService _beatLikeService;
        private readonly UserManager<User> _userManager;

        public BeatLikesController(IBeatLikeService beatLikeService, UserManager<User> userManager)
        {
            _beatLikeService = beatLikeService;
            _userManager = userManager;
        }

        [HttpPost("add-favorite-beat/{beatId}")]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(Guid beatId)
        {
            var userId = GetUserId();
            await _beatLikeService.AddToFavoritesAsync(userId, beatId);
            return Ok(new {message = "Бит добавлен в избранное."});
        }

        [HttpDelete("remove-favorite-beat/{beatId}")]
        [Authorize]
        public async Task<ActionResult> RemoveFromFavorites(Guid beatId)
        {
            var userId = GetUserId();
            await _beatLikeService.RemoveFromFavoritesAsync(userId, beatId);
            return Ok(new { message = "Бит удален из избранного." });
        }

        [HttpGet("all-favorite-beats")]
        [Authorize]
        public async Task<ActionResult<List<FavoriteBeatDto>>> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await _beatLikeService.GetFavoriteBeatsAsync(userId);
            return Ok(favorites);
        }

        [HttpGet("check-favorite/{beatId}")]
        [Authorize]
        public async Task<IActionResult> CheckIfFavorite(Guid beatId)
        {
            var userId = GetUserId();
            var isFavorite = await _beatLikeService.CheckIfFavoriteAsync(userId, beatId);
            return Ok(isFavorite);
        }

        private Guid GetUserId()
        {
            return Guid.Parse(_userManager.GetUserId(User));
        }
    }
}
