using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class UserNotFoundException : ServiceException
    {
        public UserNotFoundException() : base(ExceptionMessages.UserNotFound) { }
    }
}
