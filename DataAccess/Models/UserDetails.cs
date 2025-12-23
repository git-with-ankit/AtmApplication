using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    internal sealed class UserDetails
    {
        public string Username { get; set; }
        public int Pin { get; set; }
        public UserRole Role { get; set; }
        public bool IsFreezed { get; set; }
    }
}
