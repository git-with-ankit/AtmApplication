using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class MaxLoginAttemptsExceededException : ServiceException
    {
        public MaxLoginAttemptsExceededException() : base("Maximum login attempts exceeded. Account frozen.") { }
    }
}
