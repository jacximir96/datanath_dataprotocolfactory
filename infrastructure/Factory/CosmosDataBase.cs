using domain.Entities;
using domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Factory
{
    public class CosmosDataBase : IConnectioDataBaseDomain
    {        
        private CosmosClient client;
      
        
        public object GetObjectDataBase(object conn)
        {
            if (client==null) 
            {
                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway, // Required for Emulator
                    HttpClientFactory = () =>
                    {
                        HttpMessageHandler handler = new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                        return new HttpClient(handler);
                    },
                };
                Connection _conn =(Connection)conn;
                client = new CosmosClient(_conn.server, _conn.password, options);
            }
          
            return client;
        }
    }
}
