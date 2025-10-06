using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities.ExtracConnections
{
    public class ExtractConfiguration
    {
        public ExtractConnection connection { get; set; }
        public List<Entity> entities { get; set; }
    }
}
