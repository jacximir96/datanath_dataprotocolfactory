using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class SubRequestCompleted
    {
        public string Event { get; set; }
        public string SubRequestId { get; set; }
        public string RequestId { get; set; }
        public int RowsExtracted { get; set; }
        public int RowsLoaded { get; set; }
        public string TimeStamp { get; set; }
    }
}
