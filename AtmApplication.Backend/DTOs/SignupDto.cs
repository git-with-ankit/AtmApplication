
namespace AtmApplication.Backend.DTOs
{
    public sealed class SignupDto
    {
        public string? Username { get; set; }
        public int Pin { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
