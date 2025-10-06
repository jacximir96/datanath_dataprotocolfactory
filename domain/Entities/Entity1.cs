using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Entity1
    {
        public string name { get; set; }
        public string toName { get; set; }
        public List<PropertyTarget> properties { get; set; }
        public List<Filter> filters { get; set; }

    }
}
