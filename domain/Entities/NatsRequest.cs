using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class NatsRequest
    {
        public string @event{ get;set; }
        public string requestId { get; set; }
        public List<string> references { get; set; }
        public DateTime timestamp { get; set; }
    }
}
