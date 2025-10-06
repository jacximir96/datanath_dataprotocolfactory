using domain.Entities;
using domain.Entities.Collections;
using domain.Interfaces;
using domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class CollectionStoreRepository: ICollectionStoreRepository<CollectionStore>
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionFactoryDomain _connection;

        public CollectionStoreRepository(IConfiguration configuration, IConnectionFactoryDomain connection) 
        { 
            _configuration = configuration;
            _connection = connection;
        }  

        public async Task<List<CollectionStore>> GetCollectionStore(string dataBase, Connection connection)
        {
            List<CollectionStore> collections = new List<CollectionStore>();
            try
            {
                using (CosmosClient cosmos = (CosmosClient)_connection.CreateConnection(dataBase).GetObjectDataBase(connection))
                {
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("storescontainer").Value);
                    using (var query = container.GetItemQueryIterator<CollectionStore>())
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
