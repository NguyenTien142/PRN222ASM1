using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;
using System.Threading.Tasks;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Route("category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryServices _categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        public async Task<IActionResult> IndexCategory()
        {
            var categories = await _categoryServices.GetAllCategory();
            return View("IndexCategory", categories);
        }
    }
}
