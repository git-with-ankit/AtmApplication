using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class AccountFrozenException : ServiceException
    {
        public AccountFrozenException() : base("Account is frozen due to multiple failed attempts.") { }
    }
}
