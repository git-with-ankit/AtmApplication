using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class PinChangeDto
    {
        public string Username { get; set; } = string.Empty;
        public int CurrentPin { get; set; }
        public int NewPin { get; set; }
    }
}
