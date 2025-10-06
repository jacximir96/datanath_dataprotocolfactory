using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Entities.Collections
{
    public class CollectionStore
    {
        public string serviceDesk { get; set; }
        public List<Store> stores { get; set; }
    }
}
