using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class SubRequestRepository : ISubRequestRepository<SubRequest>
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionFactoryDomain _connection;

        public SubRequestRepository(IConfiguration configuration, 
            IConnectionFactoryDomain connection) 
        { 
            _configuration = configuration;
            _connection = connection;   
        }

        public async Task<string> Create(SubRequest subrequest, Connection connection)
        {
            string subrequestId = string.Empty;
            try
            {
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("subrequestcontainer").Value);
                    var res = await container.CreateItemAsync(subrequest, new PartitionKey(subrequest.id));
                    subrequestId=res.Resource.id;

                }
            }
            catch(Exception e) 
            {
                e.Message.ToString();
            }

            return subrequestId;
        }

        public async Task Update(SubRequest subrequest, Connection connection)
        {
            try
            {
                
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {                
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("subrequestcontainer").Value);

                    await container.ReplaceItemAsync(
                                    item: subrequest,
                                    id: subrequest.id,
                    partitionKey: new PartitionKey(subrequest.id));
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();               
            }
        }
    }
}
