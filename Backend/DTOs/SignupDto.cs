using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class SignupDto
    {
        public string Username { get; set; }
        public int Pin { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
