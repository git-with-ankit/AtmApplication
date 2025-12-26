using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class FrozenAccountsListDto
    {
        public List<FrozenAccountDto> FrozenAccounts { get; set; } = new List<FrozenAccountDto>();
    }

    public sealed class FrozenAccountDto
    {
        public string Username { get; set; } = string.Empty;
        public double Balance { get; set; }
    }
}
