using AtmApplication.DataAccess.Entities;

namespace AtmApplication.Backend.DTOs
{
    public sealed class TransactionResponseDto
    {
        public string? Username { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
