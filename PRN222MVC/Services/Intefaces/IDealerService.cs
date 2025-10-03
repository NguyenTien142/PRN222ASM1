using BusinessObject.BusinessObject.DealerModels.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Intefaces
{
    public interface IDealerService
    {
        Task<GetDealerRespond> GetDealerById(int dealerId);
    }
}
