using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class InvalidCredentialsException : ServiceException
    {
        public InvalidCredentialsException() : base(ExceptionMessages.InvalidCredentials) { }
    }
}
