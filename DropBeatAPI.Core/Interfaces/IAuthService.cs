using DropBeatAPI.Core.DTOs.LogIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto registerDto);
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<AuthResponseDto> LoginWithGoogle(GoogleLoginDto googleLoginDto);
        Task<AuthResponseDto> ConfirmEmail(ConfirmEmailDto dto);
        Task<bool> UserExists(string email);
        Task ResendConfirmationCode(ResendEmailConfirmationDto dto);
    }
}
