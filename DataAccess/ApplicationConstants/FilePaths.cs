using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ApplicationConstants
{
    internal static class FilePaths
    {
        public static readonly string DataDirectory = Path.Combine(AppContext.BaseDirectory, "DataFiles");
        public static readonly string UsersFilePath = Path.Combine(DataDirectory, "UserDetails.txt");
        public static readonly string TransactionsFilePath = Path.Combine(DataDirectory, "TransactionDetails.txt");
        public static readonly string AccountsFilePath = Path.Combine(DataDirectory, "AccountDetails.txt");
        public static readonly string AtmFilePath = Path.Combine(DataDirectory, "AtmDetails.txt");
    }
}
