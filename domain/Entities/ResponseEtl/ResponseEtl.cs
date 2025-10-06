using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace domain.Entities.ResponseEtl
{
    public class ResponseEtl
    {
        public string syncId { get; set; }
        public Data data { get; set; }
        public object cacheId { get; set; }
    }
}
