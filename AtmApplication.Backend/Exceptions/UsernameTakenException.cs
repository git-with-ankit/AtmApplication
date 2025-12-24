using AtmApplication.Backend.ApplicationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.Exceptions
{
    public class UsernameTakenException : ServiceException
    {
        public UsernameTakenException() : base(ExceptionMessages.UsernameTaken) { }
    }
}
