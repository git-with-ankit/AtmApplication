using Backend.ApplicationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class AccountFrozenException : ServiceException
    {
        public AccountFrozenException() : base(ExceptionMessages.AccountFrozen) { }
    }
}
