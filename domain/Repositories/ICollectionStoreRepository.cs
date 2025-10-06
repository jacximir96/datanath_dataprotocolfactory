using domain.Entities;
using domain.Entities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface ICollectionStoreRepository<T> where T : class
    {
        Task<List<T>> GetCollectionStore(string dataBase, Connection connection);
    }
}
