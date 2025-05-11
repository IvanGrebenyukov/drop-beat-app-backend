using DropBeatAPI.Core.DTOs.Admin;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSellerRequests()
        {
            var requests = await _adminService.GetSellerRequestsAsync();
            return Ok(requests);
        }

        [HttpPost("{userId}/approve")]
        public async Task<IActionResult> ApproveSellerRequest(Guid userId, [FromBody] AdminDecisionDto decisionDto)
        {
            var result = await _adminService.ApproveSellerRequestAsync(userId, decisionDto.AdminComment);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("{userId}/reject")]
        public async Task<IActionResult> RejectSellerRequest(Guid userId, [FromBody] AdminDecisionDto decisionDto)
        {
            var result = await _adminService.RejectSellerRequestAsync(userId, decisionDto.AdminComment);
            return result ? Ok() : BadRequest();
        }
    }

}
