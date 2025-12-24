using AtmApplication.Backend.ApplicationConstants;
using System;

namespace AtmApplication.Backend.Exceptions
{
    public class UserNotFoundException : ServiceException
    {
        public UserNotFoundException() : base(ExceptionMessages.UserNotFound) { }
    }
}
