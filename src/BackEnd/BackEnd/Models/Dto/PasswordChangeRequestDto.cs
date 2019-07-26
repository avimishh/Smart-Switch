using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Dto
{
    public class PasswordChangeRequestDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
