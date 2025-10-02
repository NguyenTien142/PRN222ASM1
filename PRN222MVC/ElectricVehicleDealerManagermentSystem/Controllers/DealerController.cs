using Microsoft.AspNetCore.Mvc;
using Services.Intefaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class DealerController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IVehicleServices _vehicleServices;
        private readonly ILogger<DealerController> _logger;

        public DealerController(IInventoryService inventoryService, IVehicleServices vehicleServices, ILogger<DealerController> logger)
        {
            _inventoryService = inventoryService;
            _vehicleServices = vehicleServices;
            _logger = logger;
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
                
                // Temporarily bypass role check for debugging
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
                return View();
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