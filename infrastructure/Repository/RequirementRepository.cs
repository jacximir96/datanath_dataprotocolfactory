using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using domain.Entities;
using domain.Entities.Collections;
using domain.Entities.LoadsConnections;
using domain.Interfaces;
using domain.Repositories;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Azure.Core.HttpHeader;
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
            List<Requirement> transactions = new List<Requirement>();
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
                            transactions.AddRange(_response.ToList());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return transactions.FirstOrDefault();
        }

        public async Task Update(Requirement requirement, Connection connection)
        {
            try
            {

                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("subrequestcontainer").Value);

                    await container.ReplaceItemAsync(
                                    item: requirement,
                                    id: requirement.id,
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
