using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.LogIn
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public bool RequiresEmailConfirmation { get; set; }
        public string Role { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Message { get; set; }
    }
}
