using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class LoginDTO
    {
        public string Username { get; set; }
        public int Pin { get; set; }
    }
}
