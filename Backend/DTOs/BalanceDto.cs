using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class BalanceDto
    {
        public string Username { get; set; } 
        public decimal Balance { get; set; }
    }
}
