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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace infrastructure.Adapter
{
    public class TestConnection : ITestConnection, IResponseDomain
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
                    response.Message = ResourceInfra.SqlTestConnectionOk;
                    return response;
                }
            }
            catch (Exception e)
            {
                response.Error = true;
                response.Message = ResourceInfra.SqlTestConnectionError;
                return response;
            }

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
                    response = GetResponse(ResourceInfra.CosmosTestConnectionOk, true);
                    return response;    

                }
              
            }
            catch (Exception e) 
            {
                response = GetResponse(ResourceInfra.CosmosTestConnectionError, false);             
                return response;
            }
        }

        public async Task<ResponseDomain> GetBlobStorageConnection(Connection connection) 
        {
            ResponseDomain response = null; 
            try
            {
                BlobContainerClient client = (BlobContainerClient)_connectionFactoryDomain.CreateConnection(connection.adapter).GetObjectDataBase(connection);
                response = GetResponse(ResourceInfra.TestConnectionBlobStirageOk,true);
                return response;
            }
            catch (Exception e) 
            {
                response = GetResponse(e.Message,false);
                return response;
            }  
        }

        public ResponseDomain GetResponse(string message, bool isSuccess)
        {
            ResponseDomain response = new ResponseDomain();
            if (isSuccess)
            {
                response.Message = message;
                response.StatusCode = HttpStatusCode.OK;
                response.Error = false;
            }
            else
            {
                response.Message = message;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Error = true;
            }

            return response;
        }
    }
}
