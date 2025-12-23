using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class LoginResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public bool IsFrozen { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
