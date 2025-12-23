using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public sealed class TransactionsHistoryDto
    {
        public List<TransactionHistoryItemDto> Transactions { get; set; } = new();
    }

    public sealed class TransactionHistoryItemDto
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
