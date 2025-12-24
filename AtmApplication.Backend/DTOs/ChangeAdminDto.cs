using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.DTOs
{
    public sealed class ChangeAdminDto
    {
        public string CurrentAdminUsername { get; set; } = string.Empty;
        public string NewAdminUsername { get; set; } = string.Empty;
        public int NewAdminPin { get; set; }
    }
}
