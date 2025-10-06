using domain.Entities;
using domain.Entities.LoadsConnections;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class TargetConfigRepository : ITargetConfigRepository
    {

        private readonly IConnectionFactoryDomain _connection;
        private readonly IConfiguration _configuration;
        public TargetConfigRepository(IConnectionFactoryDomain connection, IConfiguration configuration) 
        {
            _connection = connection;
            _configuration = configuration;

        }

   
        public async Task<LoadConnection> GetLoadConnection(Connection connection)
        {
            List<LoadConnection> collections = new List<LoadConnection>();
            try
            {
                
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var query = new QueryDefinition("SELECT * FROM c");
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("targetcontainer").Value);
                    using (var result = container.GetItemQueryIterator<LoadConnection>(query))
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
            return collections.FirstOrDefault();
        }

        public async Task<List<dynamic>> GetLoadConfiguration(Connection connection)
        {
            List<dynamic> transactions = new List<dynamic>();
            try
            {
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var query = new QueryDefinition("SELECT * FROM c");
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("targetcontainer").Value);
                    using (var result = container.GetItemQueryIterator<dynamic>(query))
                    {

                        while (result.HasMoreResults)
                        {
                            var _response = await result.ReadNextAsync();
                            transactions.AddRange(_response.ToList());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return transactions;
        }
    }
}
