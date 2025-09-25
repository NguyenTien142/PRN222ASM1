using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Route("vehicle")]
    public class VehicleController : Controller
    {
        private readonly IVehicleServices _vehicleServices;

        public VehicleController(IVehicleServices vehicleServices)
        {
            _vehicleServices = vehicleServices;
        }

        public async Task<IActionResult> IndexVehicle()
        {
            var vehicles = await _vehicleServices.GetAllVehicle();

            return View("IndexVehicle", vehicles);
        }

        [HttpGet("detail/{vehicleId}")]
        public async Task<IActionResult> VehicleDetail(int vehicleId)
        {
            var vehicle = await _vehicleServices.GetVehicleById(vehicleId);
            return View("VehicleDetail", vehicle);
        }
    }
}
