using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<int> GetTotalStockQuantityByDealerAsync(int dealerId);
    }
}
