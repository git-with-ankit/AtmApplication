using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess.Interfaces
{
    public interface ITransactionRepository : IRepository<TransactionDetails>
    {
        Task<TransactionDetails?> GetDataByUsernameAsync(string username);
        Task AddDataAsync(TransactionDetails entity);
    }
}
