using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Interfaces
{
    internal interface ITransactionRepository<T> : IReadRepository<T>, IAddRepository<T> where T : class
    {
        Task<List<T>> GetTransactionsByUsernameAsync(string username, int count);
        Task<List<T>> GetLastTransactionsAsync(int count);
    }
}
