using Backend.ApplicationConstants;
using System;

namespace Backend.Exceptions
{
    public class UserNotFoundException : ServiceException
    {
        public UserNotFoundException() : base(ExceptionMessages.UserNotFound) { }
    }
}
