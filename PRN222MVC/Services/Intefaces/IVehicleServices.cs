using BusinessObject.BusinessObject.VehicleModels.Request;
using BusinessObject.BusinessObject.VehicleModels.Respond;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Intefaces
{
    public interface IVehicleServices
    {
        Task<List<GetVehicleRespond>> GetAllVehicle();

        Task<GetDetailVehicleRespond?> GetVehicleById(int vehicleId);

        Task<List<GetVehicleByDealerRespond>> GetVehicleByDealerId(int dealerId);

        Task<bool> AddVehicle(CreateVehicleRequest vehicle);

        Task<bool> UpdateVehicle(int vehicleId, UpdateVehicleRequest vehicle);

        Task<bool> DeleteVehicle(int vehicleId);

        Task<List<GetAdminVehicleResponse>> GetVehiclesByInventoryIdAsync(int inventoryId);

        Task UpdateVehicleInventoryAsync(UpdateVehicleInventoryRequest request);
        Task DeleteVehicleInventoryAsync(int id);

        // Keep original signature without CancellationToken
        Task<ImportVehicleResponse> ImportVehiclesAsync(IFormFile file);

        // ✅ NEW: Add single vehicle with inventory
        Task<AddVehicleWithInventoryResponse> AddVehicleWithInventoryAsync(AddVehicleWithInventoryRequest request);
    }
}
