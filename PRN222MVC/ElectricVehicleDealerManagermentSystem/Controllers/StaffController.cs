using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View("StaffIndex");
        }
    }
}
