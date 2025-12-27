
namespace AtmApplication.Backend.DTOs
{
    public sealed class FrozenAccountsListDto
    {
        public List<FrozenAccountDto> FrozenAccounts { get; set; } = new List<FrozenAccountDto>();
    }

    public sealed class FrozenAccountDto
    {
        public string? Username { get; set; }
        public double Balance { get; set; }
    }
}
