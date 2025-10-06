using Azure.Storage.Blobs;
using domain.Entities;
using domain.Interfaces;
using infrastructure.Factory;
using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace infrastructure.Adapter
{
    public class TestConnection : ITestConnection
    {
      
        private readonly IConnectionFactoryDomain _connectionFactoryDomain;
        private readonly IConfiguration _configuration;

        public TestConnection(IConnectionFactoryDomain connectionFactoryDomain, IConfiguration configuration)
        { 
            _connectionFactoryDomain = connectionFactoryDomain;
            _configuration = configuration;
        }

        public async Task<ResponseDomain> GetSqlTestConnection(Connection connection)
        {
            ResponseDomain response=new ResponseDomain();
            try
            {
                
                using (SqlConnection con=(SqlConnection)_connectionFactoryDomain.CreateConnection(connection.adapter).GetObjectDataBase(connection)) 
                {
                    await con.OpenAsync();                
                    response.Error = false;
                    response.Message = "La conexión fué exitosa";
                }
            }
            catch (Exception e)
            {
                response.Error = true;
                response.Message = "Datos de conexión invalidos";
                return response;
            }

            return response;
        }

        public async Task<ResponseDomain> GetCosmosTestConnection( 
            Connection connection) 
        {
            ResponseDomain response=new ResponseDomain();
            try
            {
                using (CosmosClient cosmos = (CosmosClient)_connectionFactoryDomain.CreateConnection(connection.adapter).GetObjectDataBase(connection))
                {                   
                    var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
    .WithParameter("@id", "97a434d7-8fb6-4d02-9bcc-4359de9a237b");
                    var container = cosmos.GetDatabase(_configuration.GetSection("cosmosdb").Value).GetContainer(_configuration.GetSection("requirementcontainer").Value);
                    using (var result = container.GetItemQueryIterator<Template>(query))
                    {

                        while (result.HasMoreResults)
                        {
                            var _response = await result.ReadNextAsync();
                        }
                    }
                    response.Error = false;
                    response.Message = "conexión exitosa";

                }
                return await Task.FromResult(response);
            }
            catch (Exception e) 
            { 
                response.Error= true;
                response.Message = "Conexión invalida";
                return response;
            }
        }

        public async Task<ResponseDomain> GetBlobStorageConnection(Connection connection) 
        {
            ResponseDomain response = new ResponseDomain(); 
            try
            {
                BlobContainerClient client = (BlobContainerClient)_connectionFactoryDomain.CreateConnection(connection.adapter).GetObjectDataBase(connection);
                response.Error = false;
            }
            catch (Exception e) 
            {
                response.Error = true;
                return response;
            }  
            return response;
        }
    }
}
