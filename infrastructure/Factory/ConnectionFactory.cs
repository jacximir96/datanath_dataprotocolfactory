using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Factory
{
    public class ConnectionFactory: IConnectionFactoryDomain
    {
        private readonly IConfiguration _configuration;
        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConnectioDataBaseDomain CreateConnection(string dataBase) 
        {
            IConnectioDataBaseDomain connectioDataBase = null;    

            switch (dataBase) 
            {
                case "SqlServerSP":
                
                    connectioDataBase =new SqlServerDataBase(_configuration);
                    
                    break;

                case "MongoLocal":

                    connectioDataBase=new CosmosDataBase();

                    break;
                case "blobStorage":

                    connectioDataBase = new BlobStorageConnection(_configuration);

                    break;

            }

            return connectioDataBase;


        }
        
    }
}
