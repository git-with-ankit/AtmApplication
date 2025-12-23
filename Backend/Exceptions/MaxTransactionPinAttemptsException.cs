using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class MaxTransactionPinAttemptsExceededException : ServiceException
    {
        public MaxTransactionPinAttemptsExceededException() : base("Maximum transaction PIN attempts exceeded. Session ended.") { }
    }

}
