using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IReadRepository<T> where T : class
    {
        Task<List<T>> GetAllDataAsync();
    }
}
