using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRepository<Entity> where Entity : class
    {
        Task<List<Entity>> GetAllDataAsync();
        Task<Entity?> GetDataByUsernameAsync(string username);
        Task AddDataAsync(Entity entity);
        Task UpdateDataAsync(Entity entity);
        Task DeleteDataByUsernameAsync(string username);
    }
}
