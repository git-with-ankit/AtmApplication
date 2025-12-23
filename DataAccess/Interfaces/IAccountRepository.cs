using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    internal interface IAccountRepository<T> :IReadRepository<T>, IAddRepository<T>, IUpdateRepository<T> where T : class
    {
        Task<T?> GetDataByUsernameAsync(string username);
    }
}
