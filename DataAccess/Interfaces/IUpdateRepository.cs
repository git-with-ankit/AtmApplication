using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    internal interface IUpdateRepository<T> where T : class
    {
        Task UpdateDataAsync(T model);
    }
}
