using DropBeatAPI.Core.DTOs.Profile;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<User> _userManager;

        public ProfileController(IProfileService profileService, UserManager<User> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetProfileAsync(userId);
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileUpdateDto profileDto, IFormFile? avatarFile)
        {
            var userId = GetUserId();
            return await _profileService.UpdateProfileAsync(userId, profileDto, avatarFile) ? Ok() : BadRequest();
        }

        [HttpGet("social-links")]
        [Authorize]
        public async Task<IActionResult> GetSocialLinks()
        {
            var userId = GetUserId();
            var socialLinks = await _profileService.GetSocialLinksAsync(userId);
            return Ok(socialLinks);
        }

        [HttpPut("social-links")]
        [Authorize]
        public async Task<IActionResult> UpdateSocialLinks([FromBody] List<UserSocialLinkDto> socialLinks)
        {
            var userId = GetUserId();
            var result = await _profileService.UpdateSocialLinksAsync(userId, socialLinks);
            if (!result) return BadRequest("Не удалось обновить социальные сети.");

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPublicProfile(Guid userId)
        {
            var profile = await _profileService.GetPublicProfileAsync(userId);
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpGet("seller-status")]
        public async Task<IActionResult> GetSellerRequestStatus()
        {
            var userId = GetUserId();
            return Ok(await _profileService.GetSellerRequestStatusAsync(userId));
        }

        [HttpPost("seller-request")]
        public async Task<IActionResult> SubmitSellerRequest()
        {
            var userId = GetUserId();
            return await _profileService.SubmitSellerRequestAsync(userId) ? Ok() : BadRequest();
        }


        [AllowAnonymous]
        [HttpGet("{userId}/beats")]
        public async Task<IActionResult> GetBeatsBySeller(Guid userId)
        {
            var beats = await _profileService.GetBeatsBySellerIdAsync(userId);
            return Ok(beats);
        }

        // Получение всех моих битов (продавец)
        [Authorize(Roles = "Seller")]
        [HttpGet("my-beats")]
        public async Task<IActionResult> GetMyBeats()
        {
            var userId = GetUserId();
            var beats = await _profileService.GetMyBeatsAsync(userId);
            return Ok(beats);
        }


        private Guid GetUserId()
        {
            return Guid.Parse(_userManager.GetUserId(User));
        }
    }
}
