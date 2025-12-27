using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class ExceededPinAttemptsException : ServiceException
    {
        public ExceededPinAttemptsException() : base(ExceptionMessages.ExceededPinAttempts) { }
    }
}
