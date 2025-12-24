using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess.Entities
{
    public sealed class AtmDetails
    {
        public string AdminUsername { get; set; }  
        public double TotalBalance { get; set; }
    }
}
