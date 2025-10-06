using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class PropertyTarget
    {
        public string fromName { get; set; }
        public string toName { get; set; }
        public string toType { get; set; }
        public object defaultValue { get; set; }
    }
}
