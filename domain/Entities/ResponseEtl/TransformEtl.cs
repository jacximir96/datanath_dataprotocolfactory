using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace domain.Entities.ResponseEtl
{
    public class TransformEtl
    {
        public List<Transaction> transactions { get; set; }
        //public FeBuilder feBuilder { get; set; }
        //public PubDevice pubDevice { get; set; }
        //public Promotions promotions { get; set; }
    }
}
