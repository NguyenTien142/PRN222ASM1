using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Intefaces;
using BusinessObject.BusinessObject.UserModels.Request;
using Microsoft.Extensions.Logging;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllUsers(string searchUsername = "")
        {
            var users = await _adminService.GetAllUsersAsync(searchUsername);
            ViewBag.SearchUsername = searchUsername;
            return View(users);
        }

        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, UpdateUserRequest request)
        {
            try
            {
                if (id != request.UserId)
                {
                    return NotFound();
                }

                var result = await _adminService.EditUserAsync(request);
                if (!result)
                {
                    TempData["Error"] = "Failed to update user";
                    return View(await _adminService.GetUserByIdAsync(id));
                }

                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(GetAllUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                TempData["Error"] = "An error occurred while updating the user";
                return View(await _adminService.GetUserByIdAsync(id));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(id);
                TempData["Success"] = result ? "User deleted successfully" : "Failed to delete user";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                TempData["Error"] = "Failed to delete user";
            }
            return RedirectToAction(nameof(GetAllUsers));
        }
    }
}