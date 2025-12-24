using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class LoginResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public bool IsFrozen { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
