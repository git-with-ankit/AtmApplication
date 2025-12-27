
namespace AtmApplication.Backend.DTOs
{
    public sealed class PinChangeDto
    {
        public string? Username { get; set; }
        public int CurrentPin { get; set; }
        public int NewPin { get; set; }
    }
}
