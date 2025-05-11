using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/follow")]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly UserManager<User> _userManager;

        public FollowController(IFollowService followService, UserManager<User> userManager)
        {
            _followService = followService;
            _userManager = userManager;
        }

        private Guid GetUserId() => Guid.Parse(_userManager.GetUserId(User));

        [HttpPost("{followingId}")]
        public async Task<IActionResult> Follow(Guid followingId)
        {
            var followerId = GetUserId();
            var result = await _followService.FollowUserAsync(followerId, followingId);
            return result ? Ok() : BadRequest("Нельзя подписаться.");
        }

        [HttpDelete("unfollow/{followingId}")]
        public async Task<IActionResult> Unfollow(Guid followingId)
        {
            var followerId = GetUserId();
            var result = await _followService.UnfollowUserAsync(followerId, followingId);
            return result ? Ok() : BadRequest("Подписка не найдена.");
        }

        [HttpGet("my-followers")]
        public async Task<IActionResult> GetMyFollowers()
        {
            var userId = GetUserId();
            var result = await _followService.GetFollowersAsync(userId);
            return Ok(result);
        }

        [HttpGet("my-following")]
        public async Task<IActionResult> GetMyFollowing()
        {
            var userId = GetUserId();
            var result = await _followService.GetFollowingAsync(userId);
            return Ok(result);
        }

        [HttpGet("followers/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFollowers(Guid userId)
        {
            var result = await _followService.GetFollowersAsync(userId);
            return Ok(result);
        }

        [HttpGet("following/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFollowing(Guid userId)
        {
            var result = await _followService.GetFollowingAsync(userId);
            return Ok(result);
        }

        [HttpGet("followers/search")]
        public async Task<IActionResult> SearchMyFollowers([FromQuery] string stageName)
        {
            var userId = GetUserId();
            var result = await _followService.SearchFollowersAsync(userId, stageName);
            return Ok(result);
        }

        [HttpGet("following/search")]
        public async Task<IActionResult> SearchMyFollowing([FromQuery] string stageName)
        {
            var userId = GetUserId();
            var result = await _followService.SearchFollowingAsync(userId, stageName);
            return Ok(result);
        }

        [HttpGet("check/{userId}")]
        public async Task<IActionResult> CheckIfFollowing(Guid userId)
        {
            var currentUserId = GetUserId();
            var isFollowing = await _followService.IsFollowingAsync(currentUserId, userId);
            return Ok(isFollowing);
        }
    }

}
