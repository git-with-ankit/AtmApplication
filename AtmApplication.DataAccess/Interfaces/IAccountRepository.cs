using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmApplication.DataAccess.Entities;

namespace AtmApplication.DataAccess.Interfaces
{
    public interface IAccountRepository : IRepository<AccountDetails>
    {
        Task<AccountDetails?> GetDataByUsernameAsync(string username);
        Task AddDataAsync(AccountDetails entity);
        Task UpdateDataAsync(AccountDetails entity);
    }
}
