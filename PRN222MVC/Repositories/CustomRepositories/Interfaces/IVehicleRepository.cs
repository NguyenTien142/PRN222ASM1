using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetDetailVehiclesAsync(int id);

        Task<List<Vehicle>> GetVehicleBuyDealerIdAsync(int dealerId);
    }
}
