using domain.Entities;
using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using NATS.Client.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Adapter
{
    public class SendEtlAdapter : ISendEtl, IResponseDomain
    {
        private readonly IConfiguration _configuration;
        public SendEtlAdapter(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }


        public async Task<ResponseDomain> SendMessage(Template template) 
        {
            ResponseDomain response= null;  
            try
            {
                var json = JsonConvert.SerializeObject(template);             
                var opts = new NatsOpts { Url = _configuration.GetSection("natsurl").Value };
                var nats = new NatsConnection(opts);
                await nats.PublishAsync(_configuration.GetSection("REQUEST_EXPANDED").Value, json);
                response = GetResponse(ResourceInfra.MessageRequirementOk,true);
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
