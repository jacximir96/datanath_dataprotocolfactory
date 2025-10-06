using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities.ResponseEtl
{
    public class Data
    {
        public ExtractsResponseEtl EXTRACTS { get; set; }
        public Cache CACHE { get; set; }
        //public List<TRANSFORM> TRANSFORMS { get; set; }
        //public List<LOAD> LOADS { get; set; }
    }
}
