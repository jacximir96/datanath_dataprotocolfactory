using Azure.Storage.Blobs;
using domain.Entities;
using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Factory
{
    public class BlobStorageConnection : IConnectioDataBaseDomain
    {
        private BlobContainerClient _client;
        private readonly IConfiguration _configuration;

        public BlobStorageConnection(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }

        public object GetObjectDataBase(object connection = null)
        {
            if (_client == null) 
            {
                Connection conn = (Connection)connection;
                _client = new BlobContainerClient(new Uri(conn.server+conn.sasToken));
            }
            return _client; 
        }
    }
}
