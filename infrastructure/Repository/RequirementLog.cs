using domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class RequirementLog:ITemplateLogDomain
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequirementLog> _logger;
        public RequirementLog(IConfiguration configuration, ILogger<RequirementLog> logger) 
        { 
            _configuration = configuration;
            _logger = logger;
        }

        public void GenerateLog(string message)
        {
            try
            {             
                _logger.LogCritical(message);   
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

    }
}
