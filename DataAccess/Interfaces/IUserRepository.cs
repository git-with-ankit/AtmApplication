using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess.Interfaces
{
    public interface IUserRepository : IRepository<UserDetails>
    {
        Task<UserDetails?> GetDataByUsernameAsync(string username);
        Task AddDataAsync(UserDetails entity);
        Task UpdateDataAsync(UserDetails entity);
        Task DeleteDataByUsernameAsync(string username);
    }
}
