
namespace AtmApplication.Backend.DTOs
{
    public sealed class ChangeAdminDto
    {
        public string? CurrentAdminUsername { get; set; } 
        public string? NewAdminUsername { get; set; }
        public int NewAdminPin { get; set; }
    }
}
