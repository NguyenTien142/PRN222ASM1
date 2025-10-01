using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Intefaces;
using BusinessObject.BusinessObject.VehicleModels.Request;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Route("vehicle")]
    public class VehicleController : Controller
    {
        private readonly IVehicleServices _vehicleServices;
        private readonly ICategoryServices _categoryServices;

        public VehicleController(IVehicleServices vehicleServices, ICategoryServices categoryServices)
        {
            _vehicleServices = vehicleServices;
            _categoryServices = categoryServices;
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

        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> VehicleDealer(int dealerId)
        {
            var vehicle = await _vehicleServices.GetVehicleByDealerId(dealerId);
            return View("VehicleDealer", vehicle);
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddVehicle()
        {
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();
            return View("AddVehicle");
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(CreateVehicleRequest request)
        {
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View("AddVehicle", request);
            }
            var result = await _vehicleServices.AddVehicle(request);
            if (result)
                return RedirectToAction("IndexVehicle");
            ModelState.AddModelError("", "Failed to add vehicle.");
            return View("AddVehicle", request);
        }

        [HttpGet("update/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(int vehicleId)
        {
            var vehicle = await _vehicleServices.GetVehicleById(vehicleId);
            if (vehicle == null)
                return NotFound();
            var categories = await _categoryServices.GetAllCategory();
            var updateRequest = new UpdateVehicleRequest
            {
                VehicleId = vehicle.VehicleId,
                CategoryId = categories.FirstOrDefault(c => c.Name == vehicle.Category)?.CategoryId ?? 0,
                Color = vehicle.Color,
                Price = vehicle.Price,
                ManufactureDate = vehicle.ManufactureDate,
                Model = vehicle.Model,
                Version = vehicle.Version,
                Image = vehicle.Image
            };
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();
            return View("UpdateVehicle", updateRequest);
        }

        [HttpPost("update/{vehicleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, UpdateVehicleRequest request)
        {
            request.VehicleId = vehicleId;
            var categories = await _categoryServices.GetAllCategory();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList();

            if (request.VehicleId <= 0)
            {
                ModelState.AddModelError("", "Invalid vehicle ID.");
                return View("UpdateVehicle", request);
            }
            if (!ModelState.IsValid)
            {
                return View("UpdateVehicle", request);
            }
            var result = await _vehicleServices.UpdateVehicle(vehicleId, request);
            if (result)
                return RedirectToAction("IndexVehicle");
            ModelState.AddModelError("", "Failed to update vehicle.");
            return View("UpdateVehicle", request);
        }
    }
}
