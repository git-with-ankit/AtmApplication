using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class DepositCashDto
    {
        public string AdminUsername { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
