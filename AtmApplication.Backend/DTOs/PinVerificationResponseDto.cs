
namespace AtmApplication.Backend.DTOs
{
    public sealed class PinVerificationResponseDto
    {
        public bool IsVerified { get; set; }
        public int RemainingAttempts { get; set; }
        public bool IsAccountFrozen { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
