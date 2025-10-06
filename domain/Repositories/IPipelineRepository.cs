using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Repositories
{
    public interface IPipelineRepository<T> where T : class
    {
        Task<T> GetPipeline(Connection connection); 
    }
}
