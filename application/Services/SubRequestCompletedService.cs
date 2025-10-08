using application.Interfaces;
using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.Services
{
    public class SubRequestCompletedService : ISubRequestCompleted
    {
        public async Task ReceiveSubRequestCompleted(SubRequestCompleted subReq)
        {
            try
            {
             Console.WriteLine(subReq);
              await Task.FromResult(subReq);
            }
            catch (Exception e)
            { 
            
            }
        }
    }
}
