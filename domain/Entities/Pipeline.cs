using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Pipeline
    {
        public string? code { get; set; }
        public string? syncId { get; set; }
        public int? chunkLoad { get; set; }
        public Processes processes { get; set; }
    }
}
