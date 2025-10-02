using BusinessObject.BusinessObject.DealerModels.Respond;
using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class DealerController : Controller
    {
        private readonly IOrderServices _orderService;
        private readonly IInventoryService _inventoryService;
        private readonly IVehicleServices _vehicleServices;
        private readonly ILogger<DealerController> _logger;

        public DealerController(IOrderServices orderService, IInventoryService inventoryService, IVehicleServices vehicleServices, ILogger<DealerController> logger)
        {
            _orderService = orderService;
            _inventoryService = inventoryService;
            _vehicleServices = vehicleServices;
            _logger = logger;
        }

        public IActionResult Dashboard_Dealer()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var dealerId = HttpContext.Session.GetInt32("DealerId");
                
                Console.WriteLine($"DEBUG: UserId = {userId}, DealerId = {dealerId}");
                
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }

                if (!dealerId.HasValue)
                {
                    return Json(new { success = false, message = "Dealer information not found" });
                }

                var successfulSummary = await _orderService.GetSuccessfulOrderAsync(userId.Value);
                var pendingSummary = await _orderService.GetPendingOrderAsync(userId.Value);
                var stockQuantity = await _inventoryService.GetTotalStockQuantityByDealerAsync(dealerId.Value);
                var totalEarnings = await _orderService.GetTotalEarningsByUserAsync(userId.Value);

                var stats = new
                {
                    successfulOrdersCount = successfulSummary.Count,
                    pendingOrdersCount = pendingSummary.Count,
                    stockQuantity = stockQuantity,
                    totalEarnings = totalEarnings,
                    totalSales = successfulSummary.Count + pendingSummary.Count
                };

                Console.WriteLine($"DEBUG: Returning stats = {System.Text.Json.JsonSerializer.Serialize(stats)}");

                return Json(stats); // Direct JSON response
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSuccessfulOrders()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }
                var successfulOrders = await _orderService.GetSuccessfulOrderAsync(userId.Value);

                return Json(successfulOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Exception = {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        public async Task<IActionResult> Dashboard()
        {
            try
            {
                _logger.LogInformation("=== Dealer Dashboard Access Attempt ===");
                
                // Debug cookie and session info
                var token = Request.Cookies["X-Access-Token"];
                _logger.LogInformation("Token exists: {TokenExists}", !string.IsNullOrEmpty(token));
                
                var sessionDealerId = HttpContext.Session.GetInt32("DealerId");
                _logger.LogInformation("Session DealerId: {DealerId}", sessionDealerId);

                // Extract and log role info
                var role = GetCurrentUserRole();
                _logger.LogInformation("Extracted Role: '{Role}'", role);
                
                // Check if user is DealerManager
                var isDealerManager = !string.IsNullOrEmpty(role) && (role == "DealerManager" || role.Contains("Dealer"));
                _logger.LogInformation("Is Dealer Manager Check: {Result}", isDealerManager);

                if (!isDealerManager)
                {
                    _logger.LogWarning("Role check failed - Role: '{Role}', Redirecting to login", role);
                    TempData["Error"] = $"Access denied. Your role is '{role}'. You must be a Dealer Manager to access this page.";
                    return RedirectToAction("Login", "Account");
                }

                var dealerId = GetCurrentDealerId();
                if (dealerId == null)
                {
                    _logger.LogWarning("DealerId not found in session");
                    TempData["Error"] = "Dealer information not found. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Loading dashboard for DealerId: {DealerId}", dealerId.Value);

                // Get stats for the dashboard
                var stats = await _inventoryService.GetInventoryRequestStatsAsync(dealerId.Value);
                ViewBag.Stats = stats;
                ViewBag.DealerId = dealerId.Value;
                ViewBag.UserRole = role; // Add for debugging

                _logger.LogInformation("Dashboard loaded successfully for DealerId: {DealerId}", dealerId.Value);
                
                // Use the new dashboard view with sidebar
                return View("Dashboard_Dealer");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Dealer Dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        private string? GetCurrentUserRole()
        {
            var token = Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No access token found in cookies");
                return null;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                _logger.LogInformation("Token Validity: {IsValid}, Expires: {ExpiresAt}",
                    jwtToken.ValidTo > DateTime.UtcNow, jwtToken.ValidTo);

                // Extract role even if token is expired for debugging
                var roleClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                if (roleClaim == null)
                {
                    // Try alternative role claims
                    roleClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "role") ??
                               jwtToken.Claims.FirstOrDefault(x => x.Type.Contains("role"));
                }

                var role = roleClaim?.Value;
                _logger.LogInformation("Role from token: '{Role}', Claim Type: '{ClaimType}'",
                    role, roleClaim?.Type);

                // Log all claims for debugging
                foreach (var claim in jwtToken.Claims)
                {
                    _logger.LogInformation("Claim - Type: '{Type}', Value: '{Value}'", claim.Type, claim.Value);
                }

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading JWT token");
                return null;
            }
        }





        private int? GetCurrentDealerId()
        {
            var dealerId = HttpContext.Session.GetInt32("DealerId");
            _logger.LogInformation("Retrieved DealerId from session: {DealerId}", dealerId);
            return dealerId;
        }
    }
}