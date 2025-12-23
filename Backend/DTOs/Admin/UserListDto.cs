using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs.Admin
{
    public sealed class UserListDto
    {
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsFrozen { get; set; }
        public decimal Balance { get; set; }
    }
}
