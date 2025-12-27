
namespace AtmApplication.Backend.DTOs
{
    public sealed class LoginResponseDto
    {
        public string? Username { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public bool IsFrozen { get; set; }
        public string? Message { get; set; }
    }
}
