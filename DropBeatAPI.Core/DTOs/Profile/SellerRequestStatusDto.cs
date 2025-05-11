using DropBeatAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.Profile
{
    public class SellerRequestStatusDto
    {
        public SellerRequestStatus Status { get; set; }
        public string? AdminComment { get; set; }
    }
}
