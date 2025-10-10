using application.Interfaces;
using Azure.Core.Serialization;
using domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using NATS.Client.Core;
using System;
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
        public NatsWorker(IRequirement requirement, IConfiguration config)
        {          
            _requirement = requirement;
            _config = config;         
            var opts = new NatsOpts { Url = _config.GetSection("natsurl").Value };
            _nats = new NatsConnection(opts);               
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var msg in _nats.SubscribeAsync<string>(_config.GetSection("prefixsubjects").Value, cancellationToken: stoppingToken))
                {
                    if (msg.Subject ==_config.GetSection("event_request_created").Value)
                    {
                        _requirement.GetRequirement(msg.Data);
                        await Task.Delay(1000, stoppingToken);
                    }

                }
            }
            catch (Exception e)
            { 
              Console.WriteLine(e.StackTrace);    
            }
                              
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            
            await _nats.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

    }


}
