using AtmApplication.Backend.ApplicationConstants;

namespace AtmApplication.Backend.Exceptions
{
    public class InsufficientFundsException : ServiceException
    {
        public InsufficientFundsException() : base(ExceptionMessages.InsufficientFunds) { }
    }
}
