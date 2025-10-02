using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View("ManagerIndex");
        }
    }
}
