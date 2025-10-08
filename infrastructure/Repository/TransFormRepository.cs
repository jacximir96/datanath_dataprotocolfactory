using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;


namespace infrastructure.Repository
{
    public class TransFormRepository : ITransFormRepository<Transform>  
    {
        private readonly IConnectionFactoryDomain _connection;
        private readonly IConfiguration _configuration;
        public TransFormRepository(IConnectionFactoryDomain connection, IConfiguration configuration) 
        {
            _connection = connection;
            _configuration = configuration;
        }
        public async Task<List<Transform>> GetTransforms(Connection connection)
        {
            List<Transform> collections = new List<Transform>();
            try
            {  
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("transformscontainer").Value);
                    using (var query =container.GetItemQueryIterator<Transform>())
                    {

                        while (query.HasMoreResults)
                        {
                            var _response = await query.ReadNextAsync();
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
