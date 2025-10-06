using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Connection
    {
        
        public string server { get; set; }
        public string port { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string repository { get; set; }
        public string adapter { get; set; }
        public string sasToken { get; set; }

    }
}
