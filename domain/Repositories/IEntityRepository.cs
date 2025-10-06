using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface IEntityRepository<T> where T : class
    {
        Task<List<T>> GetEntities(Connection connection);
    }
}
