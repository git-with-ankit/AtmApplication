using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class TransactionDto
    {
        public string Username { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
