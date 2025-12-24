using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess.Entities
{
    public sealed class UserDetails
    {
        public string Username { get; set; }
        public int Pin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFreezed { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
    }
}
