using domain.Entities;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;

namespace infrastructure.Repository
{
    public class PipelineRepository : IPipelineRepository<Template>
    {
        private readonly IConnectionFactoryDomain _connection;

        public PipelineRepository(IConnectionFactoryDomain connection) 
        { 
            _connection = connection;
        } 

        public async Task<Template> GetPipeline(Connection connection)
        {
            List<Template> pipelines = new List<Template>();
            try
            {                                                          
                    using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                    {
                        var query = new QueryDefinition("SELECT * FROM c");
                        var container = cosmos.GetDatabase("DemoDb").GetContainer("pipelines");
                        using (var result = container.GetItemQueryIterator<Template>(query))
                        {

                            while (result.HasMoreResults)
                            {
                                var _response = await result.ReadNextAsync();
                                pipelines.AddRange(_response.ToList());
                            }
                        }
                    }
           
                
            }
            catch (Exception e) 
            {
                e.Message.ToString();
            }
            return pipelines.FirstOrDefault();
        }
    }
}
