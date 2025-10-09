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

        public async Task<ResponseDomain> SendRequirement(Template template)
        {
            dynamic _response=null;
            try
            {             
                    using (var client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, _configuration.GetSection("urletl").Value);
                        var json = JsonConvert.SerializeObject(template);
                        var content = new StringContent(json, null, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        string j = await response.Content.ReadAsStringAsync();                       
                        ResponseDomain responseDomain = new ResponseDomain();
                        responseDomain.data = j;
                        responseDomain.Message = $"{ResourceInfra.MessageRequirement} {template.syncId} {ResourceInfra.MessageRequirementComplement}";
                        response.StatusCode=System.Net.HttpStatusCode.OK;
                }

            }
            catch (Exception e) 
            {
                e.Message.ToString();
            }

            return _response;
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
