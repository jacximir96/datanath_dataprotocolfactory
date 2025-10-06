using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface ISubRequestRepository<T> where T :class
    {
        Task<string> Create(T subrequest, Connection connection);
    }
}
