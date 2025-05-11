using DropBeatAPI.Core.DTOs.LogIn;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _authService.UserExists(registerDto.Email))
            {
                return BadRequest("Электронная почта уже существует");
            }

            var response = await _authService.Register(registerDto);
            return Ok(response);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            try
            {
                var response = await _authService.ConfirmEmail(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("resend-confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendEmailConfirmationDto dto)
        {
            try
            {
                await _authService.ResendConfirmationCode(dto);
                return Ok("Код подтверждения отправлен повторно");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
