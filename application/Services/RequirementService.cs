using application.Interfaces;
using domain.Entities;
using domain.Entities.Collections;
using domain.Entities.ExtracConnections;
using domain.Entities.LoadsConnections;
using domain.Interfaces;
using domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text.Json;


namespace application.Services
{
    public class RequirementService : IRequirement
    {
        private readonly IGenericRepository<Requirement> _repository;
        private readonly IConfiguration _configuration;
        private readonly ISubRequestRepository<SubRequest> _subRequestRepo;
        private readonly IEntityRepository<Entity1> _entityRepository;
        private ResponseDomain response = null;
        private readonly ITemplateLogDomain _templateLogDomain;
        private readonly IConnectioDataBaseDomain _connection;
        private readonly ITestConnection _testConnection;
        private readonly ISendEtl _sendEtl;
        private readonly ITransFormRepository<Transform> _transFormRepo;
        private readonly ITargetConfigRepository _targetConfigRepo;
        private readonly ICollectionStoreRepository<CollectionStore> _collectionStoreRepo;
        private readonly IMemoryCache _cache;

        public RequirementService(
            IGenericRepository<Requirement> repository,
            IConfiguration configuration,
            IEntityRepository<Entity1> entityRepository,
            ITemplateLogDomain templateLogDomain,
            IConnectioDataBaseDomain connection,
            ITestConnection testConnection,
            ISendEtl sendEtl,
            ITransFormRepository<Transform> transFormRepo,
            ITargetConfigRepository targetConfigRepo,
            ISubRequestRepository<SubRequest> subRequestRepo,
            ICollectionStoreRepository<CollectionStore> collectionStoreRepo,
            IMemoryCache cache
            ) 
        { 
            _repository = repository;
            _configuration = configuration;            
            _targetConfigRepo= targetConfigRepo;
            _entityRepository = entityRepository;
            _templateLogDomain = templateLogDomain;
            _connection = connection;
            _testConnection = testConnection;
            _sendEtl = sendEtl;
            _transFormRepo = transFormRepo; 
            _subRequestRepo = subRequestRepo;
            _collectionStoreRepo = collectionStoreRepo;
            _cache = cache;
                      
        }

        public async Task<ResponseDomain> GetRequirement(string idrequirement)
        {
            ResponseDomain response = null;
            Connection serverExtractsData = null;
            Connection serverLoadData = null;
            List<Entity1> entities = new List<Entity1>();      
            var count = 0;
            var quantity = 0;
            List<CollectionStore> collection = null;
            Requirement requirement = null;
            LoadConnection loadConnection = null;
            List<EntityTarget> entityTargets = null;

            try
            {               
                Connection connection = new Connection();
                connection.server = _configuration.GetSection("endpoint").Value;
                connection.password = _configuration.GetSection("key").Value;
                connection.adapter = _configuration.GetSection("adapter").Value;

                requirement=_cache.Get<Requirement>(_configuration.GetSection("cacherequirement").Value);
                if (requirement == null)
                {
                    requirement = await _repository.GetRequirement(idrequirement, connection);
                    _cache.Set(_configuration.GetSection("cacherequirement").Value, requirement, TimeSpan.FromSeconds(30));
                }

                collection = _cache.Get<List<CollectionStore>>(_configuration.GetSection("cachecollection").Value);
                if (collection == null)
                {
                    collection = await _collectionStoreRepo.GetCollectionStore(connection.adapter, connection);
                    _cache.Set(_configuration.GetSection("cachecollection").Value, requirement, TimeSpan.FromSeconds(30));
                }

                foreach (var c in collection)
                {
                        foreach (var s in c.stores)
                        {

                        this.response = new ResponseDomain();
                            this.response.template = SetTemplate();
                            ExtractConfiguration extractConfig = SetExtractConfiguration(s);
                            quantity = c.stores.Count;

                            foreach (var t in requirement.entities)
                            {
                                if (extractConfig.entities == null)
                                    extractConfig.entities = new List<Entity>();

                                extractConfig.entities.Add(new Entity { 
                                 filters=t.filters,
                                 name = t.name                            
                                });
                            }

                            this.response.template.processes.EXTRACTS.Add(new Extract
                            {
                                code = _configuration.GetSection("extractcode").Value,
                                withResponse = true,
                                withCache = false,
                                syncId = idrequirement,
                                configuration = extractConfig,

                            });

                            LoadConfiguration configuration = new LoadConfiguration();                            
                            connection = SetConnection(_configuration.GetSection("endpoint").Value, _configuration.GetSection("adapter").Value,"", _configuration.GetSection("key").Value,"");
                            this.response.template.processes.TRANSFORMS = await _transFormRepo.GetTransforms(connection);
                            this.response.template.processes.LOADS = SetLoads(requirement,idrequirement, configuration, this.response);


                            if (requirement.target.connection.user != "" || requirement.target.connection.password != "" || 
                            requirement.target.connection.server !="" || requirement.target.connection.adapter !="")
                            {
                               
                              this.response = await ValidateConnections(requirement.target.connection.server, requirement.target.connection.adapter, requirement.target.connection.user, 
                                  requirement.target.connection.password, requirement.target.connection.port, requirement.target.connection.sasToken,connection);
                                if(this.response.Error)
                                    return this.response;   
                                
                               this.response.template.processes.LOADS.ForEach(l =>
                                {
                                    l.configuration.connection = SetLoadConnection(l, requirement);
                                    l.configuration.entities = requirement.target.entities;
                                });

                            }
                            else 
                            {
                                var data = await _targetConfigRepo.GetLoadConfiguration(connection);
                                foreach (var item in data)
                                {
                                    if (item is JObject obj)
                                    {
                                        var loadCon = obj.GetValue("connection");
                                        var e = obj.GetValue("entities");
                                        loadConnection = loadCon.ToObject<LoadConnection>();
                                        entityTargets = e.ToObject<List<EntityTarget>>();

                                    }
                                }

                                this.response = await ValidateConnections(s.server, s.adapter, s.user, s.password, s.port,requirement.target.connection.sasToken, connection);
                               if(this.response.Error)
                                   return this.response;

                                configuration.connection = loadConnection;
                                configuration.entities = requirement.target.entities;
                                this.response.template.processes.LOADS.ForEach(l =>
                                {
                                    l.configuration = configuration;
                                });
                            }

                            Pipeline pipeline = new Pipeline();
                            pipeline.code = this.response.template.code;
                            pipeline.syncId = idrequirement; 
                            pipeline.chunkLoad=this.response.template.chunkLoad;
                            pipeline.processes=this.response.template.processes;
                     
                            string subrequestId=await _subRequestRepo.Create(SetSubRequest(idrequirement, s, pipeline), connection);
                            requirement.estado = _configuration.GetSection("status_expanded").Value;                           
                            NatsRequest request = SetNatsRequest(idrequirement, subrequestId, _configuration.GetSection("event_request_expanded").Value);

                            _templateLogDomain.GenerateLog($"{_configuration.GetSection("logmessage1").Value} {this.response.template.syncId} {_configuration.GetSection("logmessage1").Value}");
                             var res = await _sendEtl.SendRequirement(this.response.template);

                            count = count + 1;
                            if (count==quantity)                                                                                     
                                 return null;
                                                                                   
                        }                   
                }        
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
     
            return this.response;
        }

        public async Task<ResponseDomain> ValidateSqlConnection(Connection connection)
        {
            ResponseDomain res = await _testConnection.GetSqlTestConnection(connection);         
            return res;        
        }

        public async Task<ResponseDomain> ValidateCosmosConnection(Connection connection) 
        {                       
            ResponseDomain res = await _testConnection.GetCosmosTestConnection(connection);
            return res;
        }

        public async Task<ResponseDomain> ValidateBlobStorageConnection(Connection connection) 
        {
            ResponseDomain res = await _testConnection.GetBlobStorageConnection(connection);
            return res;
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

        private ExtractConfiguration SetExtractConfiguration(Store s) 
        {
            ExtractConfiguration extractConfig = new ExtractConfiguration();
            extractConfig.connection = new ExtractConnection();
            extractConfig.connection.port = s.port;
            extractConfig.connection.password = s.password;
            extractConfig.connection.user = s.user;
            extractConfig.connection.server = s.server;
            extractConfig.connection.adapter = s.adapter;
            extractConfig.connection.repository = s.repository;

            return extractConfig;   
        }

        private Connection SetConnection(string server, 
            string adapter, 
            string user, 
            string password, 
            string port
            )
        {
            Connection conn = new Connection();
            conn.server = server;
            conn.adapter = adapter; 
            conn.user = user;
            conn.password = password;   
            conn.port = port;
            return conn;    
        
        }

        private Template SetTemplate() 
        {
            this.response.template = new Template();
            this.response.template.syncId = Guid.NewGuid().ToString();
            this.response.template.code = _configuration.GetSection("maincode").Value;
            this.response.template.chunkLoad = int.Parse(_configuration.GetSection("chunkLoad").Value);
            this.response.template.withCache = bool.Parse(_configuration.GetSection("withCache").Value);
            this.response.template.withResponse = bool.Parse(_configuration.GetSection("withResponse").Value);
            this.response.template.processes = new Processes();
            this.response.template.processes.EXTRACTS = new List<Extract>();
            return this.response.template;
        }

        private LoadConnection SetLoadConnection(Load l,Requirement requirement) 
        {
            l.configuration.connection = new LoadConnection();
            l.configuration.connection.server = requirement.target.connection.server;
            l.configuration.connection.password = requirement.target.connection.password;
            l.configuration.connection.user = requirement.target.connection.user;
            l.configuration.connection.repository = requirement.target.connection.repository;
            l.configuration.connection.adapter = requirement.target.connection.adapter;
            l.configuration.connection.port = requirement.target.connection.port;
            l.configuration.connection.sasToken = requirement.target.connection.sasToken;
            return l.configuration.connection;

        }

        private List<Load> SetLoads(Requirement requirement, string idrequirement, LoadConfiguration config, ResponseDomain res) 
        {
            this.response = res;
            this.response.template.processes.LOADS = new List<Load>();
            this.response.template.processes.LOADS.Add(new Load
            {
                code = _configuration.GetSection("loadcode").Value,
                syncId = idrequirement,              
                withCache = false,
                withResponse = true,
                configuration = config
            });

            return response.template.processes.LOADS;
        }

        private SubRequest SetSubRequest(string idrequirement, Store a, Pipeline pipeline)
        {
            SubRequest subRequest = new SubRequest 
            {
                id = idrequirement + a.storeId,
                parentRequest = idrequirement,
                status = _configuration.GetSection("status1").Value,
                store = a.storeId,
                createdAt = DateTime.Now,
                pipeline = pipeline
            };

            return subRequest;  
        }

        private NatsRequest SetNatsRequest(string requirementId, string subrequestId, string eventRequest) 
        {
            NatsRequest request = new NatsRequest();
            request.@event = eventRequest;
            request.requestId = requirementId;
            request.timestamp = DateTime.Now;
            request.references = new List<string>();
            request.references.Add(subrequestId);
            return request; 
            
        }

        private async Task<ResponseDomain> ValidateConnections(
            string server,
            string adapter,
            string user,
            string password,
            string port ,
            string sasToken,
            Connection connection
            ) 
        {
            ResponseDomain res = null;
            if (adapter == "MongoLocal")
            {
                connection = SetConnection(server, adapter, user, password, port);
                res= await ValidateCosmosConnection(connection);
                this.response.Error = res.Error;
                this.response.StatusCode = res.StatusCode;
                this.response.Message = res.Message;
            }
            else
            {
                if (adapter == "SqlServerSP")
                {
                    connection = SetConnection(server, adapter, user, password, port);
                    res = await ValidateSqlConnection(connection);   
                    this.response.Error =res.Error;
                    this.response.StatusCode = res.StatusCode;  
                    this.response.Message= res.Message; 
                }

                if (adapter == "blobStorage")
                {
                    connection = SetConnection(server, adapter, user, password, port);
                    connection.sasToken = sasToken;
                    res = await ValidateBlobStorageConnection(connection);
                    this.response.Error = res.Error;
                    this.response.StatusCode = res.StatusCode;
                    this.response.Message = res.Message;
                }

            }

            return this.response;
        }
    }
}
