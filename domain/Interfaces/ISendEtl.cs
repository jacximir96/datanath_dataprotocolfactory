using domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Interfaces
{
    public interface ISendEtl
    {
        Task<ResponseDomain> SendRequirement(Template template);
    }
}
