using Repositories.Model;
using BusinessObject.BusinessObject.VehicleModels.Request;
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

        Task<List<VehicleInventory>> GetVehiclesByInventoryIdAsync(int inventoryId);

        Task UpdateVehicleInventoryAsync(UpdateVehicleInventoryRequest request);
        Task DeleteVehicleInventoryAsync(int id);

        Task<int> CreateVehicleAsync(CreateVehicleRequest request);
        Task CreateVehicleInventoryAsync(CreateVehicleInventoryRequest request);
        Task<bool> VehicleExistsAsync(string model, string color, string version);


    }
}
