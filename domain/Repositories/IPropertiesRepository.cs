using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface IPropertiesRepository<T> where T : class
    {
        Task<List<T>> GetCosmosFilters(Connection connection);
        Task<List<T>> GetSqlProperties(Connection connection);
    }
}
