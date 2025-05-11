using DropBeatAPI.Core.DTOs.Genres;
using DropBeatAPI.Core.Entities;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DropBeatAPI.Web.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        /// <summary>
        /// Создает жанры по умолчанию (если они еще не существуют).
        /// </summary>
        [HttpPost("create-default")]
        public async Task<IActionResult> CreateDefaultGenres()
        {
            await _genreService.CreateDefaultGenres();
            return Ok(new { message = "Жанры успешно созданы." });
        }

        [HttpPost("create-default-moods")]
        public async Task<IActionResult> CreateDefaultMoods()
        {
            await _genreService.CreateDefaultMoods();
            return Ok(new { message = "Жанры успешно созданы." });
        }

        /// <summary>
        /// Получает список всех жанров.
        /// </summary>
        [HttpGet("all")]
        [AllowAnonymous] // Доступно всем (даже незалогиненным)
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenres();
            return Ok(genres);
        }

        [HttpGet("all-moods")]
        [AllowAnonymous] // Доступно всем (даже незалогиненным)
        public async Task<IActionResult> GetAllMoods()
        {
            var moods = await _genreService.GetAllMoods();
            return Ok(moods);
        }

        /// <summary>
        /// Позволяет пользователю выбрать любимые жанры.
        /// </summary>
        [HttpPost("select")]
        [Authorize] // Доступ только для авторизованных пользователей
        public async Task<IActionResult> SelectGenres([FromBody] SelectGenresDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "Пользователь не авторизован." });
            }

            try
            {
                await _genreService.SelectGenres(userId, dto);
                return Ok(new { message = "Жанры успешно сохранены." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
