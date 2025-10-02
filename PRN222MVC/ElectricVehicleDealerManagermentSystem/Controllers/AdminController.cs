using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Intefaces;
using BusinessObject.BusinessObject.UserModels.Request;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IInventoryService inventoryService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "Admin";

                // Get dashboard statistics
                var allUsers = await _adminService.GetAllUsersAsync();
                var totalUsers = allUsers.Count();
                
                // Count users by different criteria
                var activeAdmins = allUsers.Count(u => u.Role == "Admin");
                var activeDealers = allUsers.Count(u => u.Role.Contains("Dealer") && !string.IsNullOrEmpty(u.DealerTypeName));
                var evnStaff = allUsers.Count(u => u.Role == "EVN staff");

                // Get pending inventory requests count
                var allRequests = await _inventoryService.GetAllInventoryRequestsAsync();
                var pendingRequests = allRequests.Count(r => r.Status == "Pending");

                // Pass statistics to view
                ViewBag.TotalUsers = totalUsers;
                ViewBag.ActiveDealers = activeDealers;
                ViewBag.AdminCount = activeAdmins;
                ViewBag.EVNStaffCount = evnStaff;
                ViewBag.PendingRequests = pendingRequests;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics");
                
                // Fallback values if error occurs
                ViewBag.Username = GetCurrentUsername() ?? "Admin";
                ViewBag.TotalUsers = 0;
                ViewBag.ActiveDealers = 0;
                ViewBag.AdminCount = 0;
                ViewBag.EVNStaffCount = 0;
                ViewBag.PendingRequests = 0;
                
                return View();
            }
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

            // Check if request is AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(user);
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

            // Check if request is AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(user);
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
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "User ID mismatch" });
                    }
                    return NotFound();
                }

                var result = await _adminService.EditUserAsync(request);
                if (!result)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Failed to update user" });
                    }
                    TempData["Error"] = "Failed to update user";
                    return View(await _adminService.GetUserByIdAsync(id));
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "User updated successfully" });
                }

                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(GetAllUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while updating the user" });
                }

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
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = result, 
                        message = result ? "User deleted successfully" : "Failed to delete user" 
                    });
                }

                TempData["Success"] = result ? "User deleted successfully" : "Failed to delete user";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Failed to delete user" });
                }

                TempData["Error"] = "Failed to delete user";
            }
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(GetAllUsers));
        }

        // API endpoint for AJAX user search
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchUsername = "")
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync(searchUsername);
                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                return Json(new { success = false, message = "Error searching users" });
            }
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