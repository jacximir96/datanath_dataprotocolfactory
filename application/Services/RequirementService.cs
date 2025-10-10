using application.Interfaces;
using domain.Entities;
using domain.Entities.Collections;
using domain.Entities.ExtracConnections;
using domain.Entities.LoadsConnections;
using domain.Interfaces;
using domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net;


namespace application.Services
{
    public class RequirementService : IRequirement
    {
        private readonly IGenericRepository<Requirement> _repository;
        private readonly IConfiguration _configuration;
        private readonly ISubRequestRepository<SubRequest> _subRequestRepo;
        private readonly IEntityRepository<Entity1> _entityRepository;
        private readonly ITemplateLogDomain _templateLogDomain;
        private readonly IConnectioDataBaseDomain _connection;
        private readonly ITestConnection _testConnection;
        private readonly ISendEtl _sendEtl;
        private readonly ITransFormRepository<Transform> _transFormRepo;
        private readonly ITargetConfigRepository _targetConfigRepo;     
        private readonly IMemoryCache _cache;
        private readonly IResponseDomain _responseDomain;

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
            IMemoryCache cache,
            IResponseDomain responseDomain
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
            _cache = cache;
            _responseDomain = responseDomain;
                      
        }

        public async Task<ResponseDomain> GetRequirement(string idrequirement)
        {
            ResponseDomain response = null;
            Connection serverExtractsData = null;
            Connection serverLoadData = null;
            List<Entity1> entities = new List<Entity1>();      
            var count = 0;
            var quantity = 0;      
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


                foreach (var c in requirement.origins)
                {

                            response = new ResponseDomain();
                            response.template = SetTemplate(response);
                            ExtractConfiguration extractConfig = SetExtractConfiguration(c);
                            quantity = requirement.origins.Count;

                            foreach (var t in requirement.entities)
                            {
                                if (extractConfig.entities == null)
                                    extractConfig.entities = new List<Entity>();

                                extractConfig.entities.Add(new Entity { 
                                 filters=t.filters,
                                 name = t.name                            
                                });
                            }

                            response.template.processes.EXTRACTS.Add(new Extract
                            {
                                code = _configuration.GetSection("extractcode").Value,
                                withResponse = true,
                                withCache = false,
                                syncId = idrequirement,
                                configuration = extractConfig,

                            });

                            LoadConfiguration configuration = new LoadConfiguration();                            
                            connection = SetConnection(_configuration.GetSection("endpoint").Value, _configuration.GetSection("adapter").Value,"", _configuration.GetSection("key").Value,"");
                            response.template.processes.TRANSFORMS = await _transFormRepo.GetTransforms(connection);
                            response.template.processes.LOADS = SetLoads(requirement,idrequirement, configuration, response);
                           if (requirement.target.connection.user != "" || requirement.target.connection.password != "" || 
                            requirement.target.connection.server !="" || requirement.target.connection.adapter !="")
                            {
                               
                              response = await ValidateConnections(requirement.target.connection.server, requirement.target.connection.adapter, requirement.target.connection.user, 
                                  requirement.target.connection.password, requirement.target.connection.port, requirement.target.connection.sasToken,connection, response);
                                if(response.Error)
                                    return response;   
                                
                               response.template.processes.LOADS.ForEach(l =>
                                {
                                    l.configuration.connection = SetLoadConnection(l, requirement);
                                    l.configuration.connection.originRepository = c.repository;
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

                                response = await ValidateConnections(c.servidor, c.adapter, c.user, c.password, c.puerto
                                    ,requirement.target.connection.sasToken, connection, response);
                               if(response.Error)
                                   return response;

                                configuration.connection = loadConnection;
                                configuration.entities = requirement.target.entities;
                                response.template.processes.LOADS.ForEach(l =>
                                {
                                    l.configuration = configuration;
                                    l.configuration.connection.originRepository = c.repository;
                                });
                            }

                            Pipeline pipeline = new Pipeline();
                            pipeline.code = response.template.code;
                            pipeline.syncId = idrequirement; 
                            pipeline.chunkLoad=response.template.chunkLoad;
                            pipeline.processes=response.template.processes;
                     
                            string subrequestId=await _subRequestRepo.Create(SetSubRequest(idrequirement,requirement.Client ,c, pipeline), connection);
                            requirement.status = _configuration.GetSection("status_expanded").Value;                           
                            NatsRequest request = SetNatsRequest(idrequirement, subrequestId, _configuration.GetSection("event_request_expanded").Value);

                            _templateLogDomain.GenerateLog($"{ResourceApp.LogMessageRequirement} {response.template.syncId} {ResourceApp.LogMessageRequirementComplement}");
                             var res = await _sendEtl.SendMessage(response.template);

                            await _repository.Update(requirement, connection);
                            _templateLogDomain.GenerateLog($"{ResourceApp.RequirementUpdate} {requirement.id} {ResourceApp.RequirementUpdateComplement}");
                           
                            count = count + 1;
                            if (count==quantity)                                                                                     
                                 return null;
                                                                                                             
                }        
            }
            catch (Exception e)
            {
                response = _responseDomain.GetResponse(e.Message,false);
                return response;
                 
            }
     
            return response;
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


        private ExtractConfiguration SetExtractConfiguration(Origin s) 
        {
            ExtractConfiguration extractConfig = new ExtractConfiguration();
            extractConfig.connection = new ExtractConnection();
            extractConfig.connection.port = s.puerto;
            extractConfig.connection.password = s.password;
            extractConfig.connection.user = s.user;
            extractConfig.connection.server = s.servidor;
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

        private Template SetTemplate(ResponseDomain response) 
        {
            response.template = new Template();
            response.template.syncId = Guid.NewGuid().ToString();
            response.template.code = _configuration.GetSection("maincode").Value;
            response.template.chunkLoad = int.Parse(_configuration.GetSection("chunkLoad").Value);
            response.template.withCache = bool.Parse(_configuration.GetSection("withCache").Value);
            response.template.withResponse = bool.Parse(_configuration.GetSection("withResponse").Value);
            response.template.processes = new Processes();
            response.template.processes.EXTRACTS = new List<Extract>();
            return response.template;
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
            l.configuration.connection.originRepository = requirement.target.connection.originRepository;
            return l.configuration.connection;

        }

        private List<Load> SetLoads(Requirement requirement, 
            string idrequirement, 
            LoadConfiguration config, 
            ResponseDomain response) 
        {
            
            response.template.processes.LOADS = new List<Load>();
            response.template.processes.LOADS.Add(new Load
            {
                code = _configuration.GetSection("loadcode").Value,
                syncId = idrequirement,              
                withCache = false,
                withResponse = true,
                configuration = config
            });

            return response.template.processes.LOADS;
        }

        private SubRequest SetSubRequest(string idrequirement,string client, 
            Origin a, Pipeline pipeline)
        {
            SubRequest subRequest = new SubRequest 
            {
                id = idrequirement + client,
                parentRequest = idrequirement,
                status = _configuration.GetSection("status1").Value,
                store = client,
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
            Connection connection,
            ResponseDomain response
            ) 
        {
            ResponseDomain res = null;
            if (adapter == _configuration.GetSection("adapterMongoLocal").Value)
            {
                connection = SetConnection(connection.server,connection.adapter,connection.user,connection.password,connection.port);
                res= await ValidateCosmosConnection(connection);
                response.Error = res.Error;
                response.StatusCode = res.StatusCode;
                response.Message = res.Message;
            }
            else
            {
                if (adapter == _configuration.GetSection("adapterSqlServerSP").Value)
                {
                    connection = SetConnection(server, adapter, user, password, port);
                    res = await ValidateSqlConnection(connection);   
                    response.Error =res.Error;
                    response.StatusCode = res.StatusCode;  
                    response.Message= res.Message; 
                }

                if (adapter == _configuration.GetSection("adapterblobStorage").Value)
                {
                    connection = SetConnection(server, adapter, user, password, port);
                    connection.sasToken = sasToken;
                    res = await ValidateBlobStorageConnection(connection);
                    response.Error = res.Error;
                    response.StatusCode = res.StatusCode;
                    response.Message = res.Message;
                }

            }

            return response;
        }
    }
}
