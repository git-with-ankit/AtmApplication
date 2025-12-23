using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    internal interface IAtmRepository<T> : IUpdateRepository<T> where T : class
    {
        Task<double> GetBalanceAsync();
    }
}
