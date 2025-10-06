using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class EntityRepository : IEntityRepository<Entity1>
    {

        private readonly IConnectionFactoryDomain _connection;
        private readonly IConfiguration _configuration;

        public EntityRepository(IConnectionFactoryDomain connection, IConfiguration configuration) 
        {
            _connection = connection;
            _configuration = configuration;
        }

        
        public async Task<List<Entity1>> GetEntities(Connection connection)
        {
            List<Entity1> collections = new List<Entity1>();

            try
            {
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var query = new QueryDefinition("SELECT * FROM c");
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("entitiescontainer").Value);
                    using (var result = container.GetItemQueryIterator<Entity1>(query))
                    {

                        while (result.HasMoreResults)
                        {
                            var _response = await result.ReadNextAsync();
                            collections.AddRange(_response.ToList());
                        }

                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return collections;
        }
    }
}
