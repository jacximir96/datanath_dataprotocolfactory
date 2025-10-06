using domain.Entities;
using domain.Entities.LoadsConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface ITargetConfigRepository
    {
        Task<LoadConnection> GetLoadConnection(Connection connection);
        Task<List<dynamic>> GetLoadConfiguration(Connection connection);
    }
}
