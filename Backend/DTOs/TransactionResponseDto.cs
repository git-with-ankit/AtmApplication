using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class TransactionResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
