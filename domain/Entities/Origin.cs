using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Origin
    {
        public string servidor { get; set; }
        public string puerto { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string repository { get; set; }
        public string adapter { get; set; }
    }
}
