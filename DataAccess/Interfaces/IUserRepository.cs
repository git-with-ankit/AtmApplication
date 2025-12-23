using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    internal interface IUserRepository<T>: IReadRepository<T>, IAddRepository<T>, IUpdateRepository<T>, IDeleteRepository<T> where T : class
    {
        Task<T?> GetDataByUsernameAsync(string username);
    }
}
