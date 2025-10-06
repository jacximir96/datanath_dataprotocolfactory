using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Interfaces
{
    public interface ITestConnection
    {
        Task<ResponseDomain> GetSqlTestConnection(Connection connection);
        Task<ResponseDomain> GetCosmosTestConnection(Connection connection);
        Task<ResponseDomain> GetBlobStorageConnection(Connection connection);
    }
}
