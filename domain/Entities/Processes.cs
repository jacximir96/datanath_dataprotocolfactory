using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Processes
    {
        public List<Extract>? EXTRACTS { get; set; }
        public List<Transform>? TRANSFORMS { get; set; }
        public List<Load>? LOADS { get; set; }
        public List<object> PUBLISHERS { get; set; } = new List<object>();

    }
}
