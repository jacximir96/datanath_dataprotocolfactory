using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class SubRequest
    {
        public string id { get; set; }
        public string parentRequest { get; set; }
        public string store { get; set; }
        public Pipeline pipeline { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }

    }
}
