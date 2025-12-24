using Backend.ApplicationConstants;
using System;

namespace Backend.Exceptions
{
    public class AdminFreezeException : ServiceException
    {
        public AdminFreezeException() : base(ExceptionMessages.AdminFreezeAttempt) { }
    }
}
