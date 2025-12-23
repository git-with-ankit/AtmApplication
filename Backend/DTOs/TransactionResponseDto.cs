using DataAccess.Models;
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
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
