using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Intefaces;
using BusinessObject.BusinessObject.UserModels.Request;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessObject.BusinessObject.OrderModels.Respond;
using BusinessObject.BusinessObject.ReportModels;
using BusinessObject.BusinessObject.OrderModels.Response;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderServices _orderServices;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IInventoryService inventoryService, IOrderServices orderServices, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _inventoryService = inventoryService;
            _orderServices = orderServices;
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

        // Báo cáo doanh số theo đại lý
        public async Task<IActionResult> DealerSalesReport(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "Admin";

                // Get dealer sales report data
                var dealerReports = await _adminService.GetDealerSalesReportAsync(startDate, endDate);
                var summary = await _adminService.GetSalesReportSummaryAsync(startDate, endDate);

                ViewBag.DealerReports = dealerReports;
                ViewBag.Summary = summary;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dealer sales report");

                // Fallback values if error occurs
                ViewBag.Username = GetCurrentUsername() ?? "Admin";
                ViewBag.DealerReports = new List<DealerSalesReportResponse>();
                ViewBag.Summary = new SalesReportSummary
                {
                    TotalDealers = 0,
                    TotalOrders = 0,
                    TotalEarnings = 0,
                    ReportGeneratedDate = DateTime.Now
                };
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View();
            }
        }

        // API endpoint for AJAX dealer sales report
        [HttpGet]
        public async Task<IActionResult> GetDealerSalesReportData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var dealerReports = await _adminService.GetDealerSalesReportAsync(startDate, endDate);
                var summary = await _adminService.GetSalesReportSummaryAsync(startDate, endDate);

                return Json(new
                {
                    success = true,
                    data = dealerReports,
                    summary = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dealer sales report data");
                return Json(new { success = false, message = "Error loading report data" });
            }
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

        // Order Approval Management
        public async Task<IActionResult> OrderApproval()
        {
            try
            {
                // Get username from JWT token
                var username = GetCurrentUsername();
                ViewBag.Username = username ?? "Admin";

                // Get all pending orders for approval
                var pendingOrders = await _orderServices.GetAllPendingOrdersAsync();
                ViewBag.PendingOrders = pendingOrders;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order approval page");

                // Fallback values if error occurs
                ViewBag.Username = GetCurrentUsername() ?? "Admin";
                ViewBag.PendingOrders = new List<GetPendingOrderResponse>();

                return View();
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdForApproval(id);
                if (order == null)
                {
                    return NotFound();
                }

                // Check if request is AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(order);
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order details for approval");
                return Json(new { success = false, message = "Error loading order details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            try
            {
                var result = await _orderServices.ApproveOrderAsync(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = result,
                        message = result ? "Order approved successfully" : "Failed to approve order"
                    });
                }

                TempData["Success"] = result ? "Order approved successfully" : "Failed to approve order";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order {OrderId}", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while approving the order" });
                }

                TempData["Error"] = "An error occurred while approving the order";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(OrderApproval));
        }

        [HttpPost]
        public async Task<IActionResult> RejectOrder(int id)
        {
            try
            {
                var result = await _orderServices.RejectOrderAsync(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = result,
                        message = result ? "Order rejected successfully" : "Failed to reject order"
                    });
                }

                TempData["Success"] = result ? "Order rejected successfully" : "Failed to reject order";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order {OrderId}", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while rejecting the order" });
                }

                TempData["Error"] = "An error occurred while rejecting the order";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(OrderApproval));
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