using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class TransactionsHistoryDto
    {
        public List<TransactionHistoryItemDto> Transactions { get; set; } = new();
    }

    public sealed class TransactionHistoryItemDto
    {
        public string Username { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAdminTransaction { get; set; }
    }
}
