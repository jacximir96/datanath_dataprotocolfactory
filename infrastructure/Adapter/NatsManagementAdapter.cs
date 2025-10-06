using domain.Entities;
using Microsoft.Extensions.Configuration;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Adapter
{
    public class NatsManagementAdapter
    {
        private readonly IConfiguration _config;
        public NatsManagementAdapter(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMessage(Template template, NatsRequest request)
        {
            try
            {
                var opts = new NatsOpts { Url = _config.GetSection("natsurl").Value };
                var nats = new NatsConnection(opts);
                await nats.PublishAsync(_config.GetSection("subject").Value, request);
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }
    }
}
