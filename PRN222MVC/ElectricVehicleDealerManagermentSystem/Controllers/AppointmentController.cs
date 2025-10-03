using BusinessObject.BusinessObject.Appointment.Request;
using Microsoft.AspNetCore.Mvc;
using Services.Implements;
using Services.Intefaces;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IVehicleServices _vehicleServices;

        public AppointmentController(IAppointmentService appointmentService, IVehicleServices vehicleServices)
        {
            _appointmentService = appointmentService;
            _vehicleServices = vehicleServices;
        }


        // ======================
        // GET: Appointment List (for Admin/Staff)
        // ======================
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            return View("IndexAppointment", appointments);
        }

        // ======================
        // GET: Appointment Details
        // ======================
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View("DetailsAppointment", appointment);
        }

        // ======================
        // GET: Create Appointment (Customer Form)
        // ======================
        [HttpGet]
        public IActionResult Create(int vehicleId)
        {
            var dto = new AddAppointmentRequest
            {
                VehicleId = vehicleId,
                AppointmentDate = DateTime.Now.AddDays(1)
            };
            return View("CreateAppointment", dto);
        }

        // ======================
        // POST: Create Appointment
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAppointmentRequest dto)
        {
            if (!ModelState.IsValid)
                return View("CreateAppointment", dto);

            var success = await _appointmentService.AddAppointmentAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "This vehicle is already booked at that time. Please choose another time.");
                return View("CreateAppointment", dto);
            }

            TempData["Success"] = "Your test drive appointment has been booked successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();
            await _appointmentService.CancelAppointmentAsync(id);
            TempData["Success"] = "Appointment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewVehicle()
        {
            var vehicles = await _vehicleServices.GetAllVehicle();
            return View("ViewVehicle", vehicles);
        }
    }
}
