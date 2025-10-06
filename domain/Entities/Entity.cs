using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Entity
    {
        public string name { get; set; }     
        public List<Properties> properties { get; set; }= new List<Properties>();   
        public List<Filter> filters { get; set; }
        
    }
}
