using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ApplicationConstants
{
    internal class ExceptionConstants
    {
        public const string FileReadError = "Failed to read data from {0}";
        public const string FileWriteError = "Failed to save data to {0}";
        public const string FileAccessDenied = "No permission to access storage in {0}";
        public const string ParseDataError = "Corrupt data detected in {0}";
    }
}//Fill this
