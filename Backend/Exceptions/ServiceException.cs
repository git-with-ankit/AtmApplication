using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.Backend.Exceptions
{
    public abstract class ServiceException : Exception
    {
        protected ServiceException(string message) : base(message) { }
    }
}
