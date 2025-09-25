using BusinessObject.BusinessObject.VehicleModels.Request;
using BusinessObject.BusinessObject.VehicleModels.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Intefaces
{
    public interface IVehicleServices
    {
        Task<List<GetVehicleRespond>> GetAllVehicle();

        Task<GetDetailVehicleRespond?> GetVehicleById(int vehicleId);

        Task<GetDetailVehicleRespond?> AddVehicle(GetDetailVehicleRespond vehicle);

        Task<bool> UpdateVehicle(UpdateVehicleRequest vehicle);

        Task<bool> DeleteVehicle(int vehicleId);
    }
}
