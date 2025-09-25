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
    }
}
