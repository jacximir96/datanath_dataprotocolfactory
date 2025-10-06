using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class MetricResource
    {
        public string MetricName { get; set; }
        public double ProvisionedRu { get; set; }
        public double ConsumedRu { get; set; }
    }
}
