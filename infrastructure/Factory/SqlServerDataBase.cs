using domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using domain.Entities;
using Azure.Storage.Blobs;

namespace infrastructure.Factory
{
    public class SqlServerDataBase : IConnectioDataBaseDomain
    {
        private readonly IConfiguration _configuration;
        private Connection _connection;
        
        public SqlServerDataBase(IConfiguration configuration) 
        { 
            _configuration = configuration; 
        }

        public object GetObjectDataBase(object connection=null)
        {
            _connection = (Connection)connection;
            string conn = "Server=" + _connection.server + ";Database=" + _configuration.GetSection("sqltestdatabase").Value + ";User ID=" + _connection.user + ";Password=" + _connection.password + _configuration.GetSection("sqlcomplementconnection").Value;
            SqlConnection con =new SqlConnection(conn);
            return con;
        }
    }
}
