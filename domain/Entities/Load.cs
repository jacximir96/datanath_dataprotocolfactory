using domain.Entities.LoadsConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Load
    {
        public string code { get; set; }
        public bool withResponse { get; set; }
        public bool withCache { get; set; }
        public string? syncId { get; set; }
        public LoadConfiguration? configuration { get; set; }
        public DataInput? dataInput { get; set; } = new DataInput();
        

    }
}
