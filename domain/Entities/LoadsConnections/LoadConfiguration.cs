using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities.LoadsConnections
{
    public class LoadConfiguration
    {
        public LoadConnection connection { get; set; }
        public List<EntityTarget> entities { get; set; }
    }
}
