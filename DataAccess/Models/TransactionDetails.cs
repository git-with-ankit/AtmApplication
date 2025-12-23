using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    internal sealed class TransactionDetails
    {
        public string UserName { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public double NewBalance { get; set; }
        public bool IsAdminTransaction { get; set; }
    }
}
