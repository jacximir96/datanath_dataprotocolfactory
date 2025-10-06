using domain.Entities;
using domain.Entities.ResponseEtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface ITransFormRepository<T> where T : class 
    {
        Task<List<T>> GetTransforms(Connection connection);
    }
}
