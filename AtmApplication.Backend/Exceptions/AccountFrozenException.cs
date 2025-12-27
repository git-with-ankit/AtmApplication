using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class AccountFrozenException : ServiceException
    {
        public AccountFrozenException() : base(ExceptionMessages.AccountFrozen) { }
    }
}
