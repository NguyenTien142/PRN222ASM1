using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IDealerRepository
    {
        Task<Dealer?> GetDealerByIdAsync(int dealerId);
    }
}
