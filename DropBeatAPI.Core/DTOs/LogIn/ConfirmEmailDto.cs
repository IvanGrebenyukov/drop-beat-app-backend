﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Core.DTOs.LogIn
{

    public class ConfirmEmailDto
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
