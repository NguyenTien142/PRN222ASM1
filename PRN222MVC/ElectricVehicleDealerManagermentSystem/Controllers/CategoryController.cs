using BusinessObject.BusinessObject.CategoryModels.Request;
using BusinessObject.BusinessObject.CategoryModels.Respond;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HttpGet]
        public async Task<IActionResult> IndexCategory()
        {
            var categories = await _categoryServices.GetAllCategory();
            return View("IndexCategory", categories);
        }

        [HttpGet("add")]
        public IActionResult AddCategory()
        {
            return View("AddCategory", new AddCategoryResquest());
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(AddCategoryResquest request)
        {
            if (!ModelState.IsValid)
            {
                return View("AddCategory", request);
            }

            var result = await _categoryServices.AddCategory(request);
            if (result)
                return RedirectToAction("IndexCategory");

            ModelState.AddModelError("", "Failed to add category.");
            return View("AddCategory", request);
        }

        [HttpGet("update/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId)
        {
            var category = await _categoryServices.GetCategoryById(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            var updateRequest = new UpdateCategoryRequest
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
            return View("UpdateCategory", updateRequest);
        }

        [HttpPost("update/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int categoryId, UpdateCategoryRequest request)
        {
            request.CategoryId = categoryId;
            if (!ModelState.IsValid)
            {
                return View("UpdateCategory", request);
            }
            var addRequest = new AddCategoryResquest { Name = request.Name };
            var result = await _categoryServices.UpdateCategory(categoryId, addRequest);
            if (result)
            {
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction("IndexCategory");
            }
            ModelState.AddModelError("", "Failed to update category.");
            return View("UpdateCategory", request);
        }

        [HttpPost("delete/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var result = await _categoryServices.DeleteCategory(categoryId);
            if (result)
            {
                TempData["Success"] = "Category deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete category. It may be in use by vehicles.";
            }
            return RedirectToAction("IndexCategory");
        }
    }
}
