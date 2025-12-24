using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.ApplicationConstants
{
    public static class Constants
    {
        public const int MaxPinAttempts = 3;
        public const int PinLength = 4;
        public const int DefaultTransactionHistoryCount = 5;
        public const int MinUsernameLength = 3;
        public const int MaxUsernameLength = 20;
    }
}
