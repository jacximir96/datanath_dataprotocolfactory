using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Target
    {
        public string? adapter { get; set; }
        public string? repository { get; set; }
        public string? entityTarget { get; set; }
        public Connection? connection { get; set; }
        public List<EntityTarget> entities { get; set; }

    }
}
