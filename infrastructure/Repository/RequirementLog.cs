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
       
        public void GenerateLog(string message)
        {
            try
            {
                
                using(var _file=File.OpenWrite("E:\\repo-data-protocol-factory\\dataprotocolfactory\\logs\\log.txt")) 
                {
                     byte[] bytes = Encoding.UTF8.GetBytes(message);                  
                    _file.Write(bytes, 0, bytes.Length);
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

    }
}
