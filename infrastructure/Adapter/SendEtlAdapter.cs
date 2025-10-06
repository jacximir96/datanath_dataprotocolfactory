using domain.Entities;
using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Adapter
{
    public class SendEtlAdapter : ISendEtl
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
                        responseDomain.Message = $"requerimiento {template.syncId} enviado con exito";
                        response.StatusCode=System.Net.HttpStatusCode.OK;
                }

            }
            catch (Exception e) 
            {
                e.Message.ToString();
            }

            return _response;
        }
    }
}
