using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class AttemptLimitDto
    {
        public string Message { get; set; } = string.Empty;
        public int RemainingAttempts { get; set; }
        public bool IsAccountFrozen { get; set; }
    }
}
