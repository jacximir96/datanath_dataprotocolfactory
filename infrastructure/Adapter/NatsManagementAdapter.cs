using domain.Entities;
using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Adapter
{
    public class NatsManagementAdapter:INatsManagementDomain, IResponseDomain
    {
        private readonly IConfiguration _config;
        public NatsManagementAdapter(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ResponseDomain> SendMessage(Template template, NatsRequest request)
        {
            ResponseDomain response= null;  
            try
            {
                var opts = new NatsOpts { Url = _config.GetSection("natsurl").Value };
                var nats = new NatsConnection(opts);
                await nats.PublishAsync(_config.GetSection("subject").Value, request);
                response = GetResponse(ResourceInfra.MessageSentOk,true);
                return response;
            }
            catch (Exception e)
            {
                response = GetResponse(e.Message,false);
                return response;
            }
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
    }
}
