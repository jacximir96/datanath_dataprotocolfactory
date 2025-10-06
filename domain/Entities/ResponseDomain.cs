using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class ResponseDomain
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Template template { get; set; }   
        public string data { get; set; }
        public bool Error { get; set; }
        
    }
}
