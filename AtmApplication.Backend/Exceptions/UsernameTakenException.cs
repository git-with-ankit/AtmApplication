using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class UsernameTakenException : ServiceException
    {
        public UsernameTakenException() : base(ExceptionMessages.UsernameTaken) { }
    }
}
