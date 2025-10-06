using domain.Entities;
using domain.Entities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetRequirement(string idrequirement, Connection connection);

    }
}
