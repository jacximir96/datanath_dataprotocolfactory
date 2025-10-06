using domain.Entities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Requirement
    {
        public string? id { get; set; }
        public string serviceDesk { get; set; }
        public Store stores { get; set; }
        public string entity { get; set; }
        public string operation { get; set; }
        public List<Entity1> entities { get; set; }
        public string transformationKey { get; set; }
        public Target target { get; set; }
        public string estado { get; set; }
        public Meta meta { get; set; }
    }
}
