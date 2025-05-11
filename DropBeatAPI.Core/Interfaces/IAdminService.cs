using DropBeatAPI.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.Interfaces
{
    public interface IAdminService
    {
        Task<List<SellerRequestDto>> GetSellerRequestsAsync();
        Task<bool> ApproveSellerRequestAsync(Guid userId, string adminComment);
        Task<bool> RejectSellerRequestAsync(Guid userId, string adminComment);
    }
}
