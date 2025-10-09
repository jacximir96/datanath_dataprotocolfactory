using domain.Entities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Template
    {      
        public string code { get; set; }
        public bool withResponse { get; set; }
        public bool withCache { get; set; }
        public string syncId { get; set; }
        public int chunkLoad { get; set; }
        public Processes processes { get; set; }  
        public List<string> references { get; set; }
    }
}
