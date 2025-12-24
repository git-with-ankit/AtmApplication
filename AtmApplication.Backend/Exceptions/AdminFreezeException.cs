using AtmApplication.Backend.ApplicationConstants;
using System;

namespace AtmApplication.Backend.Exceptions
{
    public class AdminFreezeException : ServiceException
    {
        public AdminFreezeException() : base(ExceptionMessages.AdminFreezeAttempt) { }
    }
}
