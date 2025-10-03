using BusinessObject.BusinessObject.Appointment.Request;
using Microsoft.AspNetCore.Mvc;
using Services.Implements;
using Services.Intefaces;
using Microsoft.AspNetCore.Authorization;
using BusinessObject.BusinessObject.Appointment.Respond;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

        public async Task<IActionResult> ViewVehicle()
        {
            var vehicles = await _vehicleServices.GetAllVehicle();
            return View("ViewVehicle", vehicles);
        }

        // ======================
        // GET: Appointment Approval (for DealerManager)
        // ======================
        [Authorize(Roles = "DealerManager")]
        public async Task<IActionResult> AppointmentApproval()
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "DealerManager";

                // Get all appointments for approval
                var allAppointments = await _appointmentService.GetAppointmentsAsync();
                ViewBag.AllAppointments = allAppointments;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Username = GetCurrentUsername() ?? "DealerManager";
                ViewBag.AllAppointments = new List<GetAppointmentRespond>();
                ViewBag.Error = "Error loading appointments";

                return View();
            }
        }

        // ======================
        // POST: Approve Appointment
        // ======================
        [HttpPost]
        [Authorize(Roles = "DealerManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAppointment(ApproveAppointmentRequest request)
        {
            try
            {
                var result = await _appointmentService.ApproveAppointmentAsync(request);
                if (!result)
                {
                    TempData["Error"] = "Failed to approve appointment. It may have already been processed.";
                }
                else
                {
                    TempData["Success"] = "Appointment approved successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while approving the appointment.";
            }

            return RedirectToAction(nameof(AppointmentApproval));
        }

        // ======================
        // POST: Reject Appointment
        // ======================
        [HttpPost]
        [Authorize(Roles = "DealerManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAppointment(RejectAppointmentRequest request)
        {
            try
            {
                var result = await _appointmentService.RejectAppointmentAsync(request);
                if (!result)
                {
                    TempData["Error"] = "Failed to reject appointment. It may have already been processed.";
                }
                else
                {
                    TempData["Success"] = "Appointment rejected successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while rejecting the appointment.";
            }

            return RedirectToAction(nameof(AppointmentApproval));
        }

        // Helper method to get current username from JWT token
        private string GetCurrentUsername()
        {
            var token = Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    return jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                }
            }
            catch
            {
                // Token parsing failed
            }

            return null;
        }
    }
}
