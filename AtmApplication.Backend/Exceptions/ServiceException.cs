
namespace AtmApplication.Backend.Exceptions
{
    public abstract class ServiceException : Exception
    {
        protected ServiceException(string message) : base(message) { }
    }
}
