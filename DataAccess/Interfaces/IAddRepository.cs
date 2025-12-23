using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    internal interface IAddRepository<T> where T : class
    {
        Task AddDataAsync(T model);
    }
}
