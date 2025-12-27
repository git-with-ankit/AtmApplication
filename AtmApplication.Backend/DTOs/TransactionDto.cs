using AtmApplication.DataAccess.Entities;

namespace AtmApplication.Backend.DTOs
{
    public sealed class TransactionDto
    {
        public string? Username { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
    }
}
