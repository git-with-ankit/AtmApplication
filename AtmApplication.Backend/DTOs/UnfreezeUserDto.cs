using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class UnfreezeUserDto
    {
        public string AdminUsername { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
