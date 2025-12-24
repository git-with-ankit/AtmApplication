using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class BalanceDto
    {
        public string Username { get; set; } 
        public double Balance { get; set; }
    }
}
