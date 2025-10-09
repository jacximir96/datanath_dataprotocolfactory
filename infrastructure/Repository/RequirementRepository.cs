using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Extensions.Configuration;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace infrastructure.Repository
{
    public class RequirementRepository : IGenericRepository<Requirement>       
    {
        private readonly IConnectionFactoryDomain _connection;
        private readonly IConfiguration _configuration;
        

        public RequirementRepository(IConnectionFactoryDomain connection, IConfiguration configuration) 
        { 
            _connection = connection;
            _configuration = configuration;
        }

      
        public async Task<Requirement> GetRequirement(string idrequirement, Connection connection)
        {
            List<Requirement> requirements = new List<Requirement>();
            try
            {             
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
    .WithParameter("@id", idrequirement);
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("requirementcontainer").Value);
                    using (var result =container.GetItemQueryIterator<Requirement>(query))
                    {

                        while (result.HasMoreResults)
                        {
                            var _response = await result.ReadNextAsync();
                            requirements.AddRange(_response.ToList());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return requirements.FirstOrDefault();
        }

        public async Task Update(Requirement requirement, Connection connection)
        {
            try
            {

                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("requirementcontainer").Value);

                    var response = await container.ReadItemAsync<Requirement>(
                            requirement.id,
                            new PartitionKey(requirement.id)
                         );

                     Requirement _requirement = response.Resource;
                    _requirement.status = requirement.status;
                    _requirement.references = requirement.references;

                    await container.ReplaceItemAsync(
                                    item: _requirement,
                                    id: _requirement.id,
                    partitionKey: new PartitionKey(requirement.id));
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();              
            }
        }

    }
}
