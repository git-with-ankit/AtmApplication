using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess
{
    public interface IRepository<Entity> where Entity : class
    {
        Task<List<Entity>> GetAllDataAsync();
    }
}
