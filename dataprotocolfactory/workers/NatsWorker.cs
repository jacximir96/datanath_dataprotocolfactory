using application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using NATS.Client.Core;
using System.Threading;
using System.Threading.Tasks;

namespace dataprotocolfactory.workers
{
    public class NatsWorker: BackgroundService
    {
        private readonly IRequirement _requirement;
        private readonly IConfiguration _config;
        private readonly INatsConnection _nats;
        public NatsWorker(IRequirement requirement, IConfiguration config)
        {          
            _requirement = requirement;
            _config = config;         
            var opts = new NatsOpts { Url = _config.GetSection("natsurl").Value };
            _nats = new NatsConnection(opts);               
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {              
                await foreach (var msg in _nats.SubscribeAsync<string>(_config.GetSection("subject_request_created").Value, cancellationToken: stoppingToken))
                {
                        _requirement.GetRequirement(msg.Data);
                        await Task.Delay(1000, stoppingToken);      
                }                               
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            
            await _nats.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

    }


}
