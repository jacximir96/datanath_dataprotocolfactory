using application.Interfaces;
using Azure.Core.Serialization;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using NATS.Client.Core;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace dataprotocolfactory.workers
{
    public class NatsWorker: BackgroundService
    {
        private readonly IRequirement _requirement;
        private readonly IConfiguration _config;
        private readonly INatsConnection _nats;
        private readonly  ISubRequestCompleted _subRequestCompleted;
        public NatsWorker(IRequirement requirement, IConfiguration config)
        {          
            _requirement = requirement;
            _config = config;         
            var opts = new NatsOpts { Url = _config.GetSection("natsurl").Value };
            _nats = new NatsConnection(opts);               
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
   
            //await foreach (var msg in _nats.SubscribeAsync<string>(_config.GetSection("subject_request_created").Value, cancellationToken: stoppingToken))
            await foreach (var msg in _nats.SubscribeAsync<string>("subject.>", cancellationToken: stoppingToken))
            {
                if (msg.Subject == "subject.REQUEST_CREATED") 
                {
                    _requirement.GetRequirement(msg.Data);
                    await Task.Delay(1000, stoppingToken);
                }

                if(msg.Subject == "subject.SUBREQUEST_COMPLETED") 
                {
                    var json= JsonSerializer.Serialize(msg.Data); 
                    SubRequestCompleted sub = JsonSerializer.Deserialize<SubRequestCompleted>(json);
                }
            }                               
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            
            await _nats.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

    }


}
