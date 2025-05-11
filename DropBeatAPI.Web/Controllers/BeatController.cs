using DropBeatAPI.Core.DTOs.Beat;
using DropBeatAPI.Core.DTOs.Tag;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeatController : ControllerBase
    {
        private readonly IBeatService _beatService;
        private readonly UserManager<User> _userManager;

        public BeatController(IBeatService beatService, UserManager<User> userManager)
        {
            _beatService = beatService;
            _userManager = userManager;
        }

        // Получение всех битов
        [HttpGet("all-beat")]
        public async Task<ActionResult<List<ShortBeatDto>>> GetAllBeats()
        {
            var beats = await _beatService.GetAllBeatsAsync();
            return Ok(beats);
        }

        // Получение короткой информации о бите по ID
        [HttpGet("{beatId}")]
        public async Task<ActionResult<ShortBeatDto>> GetShortBeatById(Guid beatId)
        {
            var beat = await _beatService.GetShortBeatByIdAsync(beatId);
            if (beat == null) return NotFound();
            return Ok(beat);
        }

        // Получение полной информации о бите по ID
        [HttpGet("{beatId}/details")]
        public async Task<ActionResult<BeatDto>> GetBeatById(Guid beatId)
        {
            var beat = await _beatService.GetBeatByIdAsync(beatId);
            if (beat == null) return NotFound();
            return Ok(beat);
        }

        // Добавление нового бита (только для пользователей с ролью Seller)
        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<Guid>> AddBeat([FromForm] CreateBeatDto dto)
        {
            var sellerId = GetUserId();
            var beatId = await _beatService.AddBeatAsync(dto, sellerId);
            return CreatedAtAction(nameof(GetBeatById), new { beatId }, beatId);
        }

        // Удаление бита (только для владельца бита)
        [HttpDelete("{beatId}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> DeleteBeat(Guid beatId)
        {
            var sellerId = GetUserId();
            var success = await _beatService.DeleteBeatAsync(beatId, sellerId);

            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("random-tags")]
        public async Task<ActionResult<List<TagDto>>> GetRandomTags()
        {
            var tags = await _beatService.GetRandomTagsAsync();
            return Ok(tags);
        }

        // Поиск тегов по строке
        [HttpGet("search-tags")]
        public async Task<ActionResult<List<TagDto>>> SearchTags([FromQuery] string query)
        {
            var tags = await _beatService.SearchTagsAsync(query);
            return Ok(tags);
        }
        
        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredBeats([FromBody] FilterBeatsDto filterDto)
        {
            var beats = await _beatService.GetFilteredBeatsAsync(filterDto);
            return Ok(beats);
        }

        private Guid GetUserId()
        {
            return Guid.Parse(_userManager.GetUserId(User));
        }
    }

}
