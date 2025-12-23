using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ApplicationConstants
{
    internal class ExceptionConstants
    {
        public const string UserAlreadyExists = "User '{0}' already exists";
        public const string AccountAlreadyExists = "Account for '{0}' already exists";
        public const string AtmRecordAlreadyExists = "ATM record for admin '{0}' already exists";
        public const string AccountNotFound = "No account found for '{0}'";
        public const string InvalidUserDetailsFormat = "Invalid user details format. Expected 4 fields.";
        public const string InvalidAccountDetailsFormat = "Invalid account details format. Expected 2 fields.";
        public const string InvalidAtmDetailsFormat = "Invalid ATM details format. Expected 2 fields.";
        public const string InvalidTransactionDetailsFormat = "Invalid transaction details format. Expected 6 fields.";
    }
}
