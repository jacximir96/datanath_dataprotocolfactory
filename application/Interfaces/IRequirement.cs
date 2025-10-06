using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.Interfaces
{
    public interface IRequirement
    {
        Task<ResponseDomain> GetRequirement(string idrequirement);     
        Task<ResponseDomain> ValidateSqlConnection(Connection connection);
        Task<ResponseDomain> ValidateBlobStorageConnection(Connection connection);
        ResponseDomain GetResponse(string message, bool isSuccess);
    }
}
