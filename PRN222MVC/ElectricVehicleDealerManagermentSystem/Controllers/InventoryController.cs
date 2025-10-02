using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject.BusinessObject.InventoryModels.Request;
using Services.Intefaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IVehicleServices _vehicleServices;
        private readonly IAdminService _adminService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, IVehicleServices vehicleServices, IAdminService adminService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _vehicleServices = vehicleServices;
            _adminService = adminService;
            _logger = logger;
        }

        // GET: Inventory/Requests - Admin view all requests
        public async Task<IActionResult> Requests()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            var requests = await _inventoryService.GetAllInventoryRequestsAsync();
            return View(requests);
        }

        // GET: Inventory/MyRequests - DealerManager view their requests
        public async Task<IActionResult> MyRequests()
        {
            var dealerId = GetCurrentDealerId();
            if (dealerId == null)
            {
                TempData["Error"] = "Dealer information not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            var requests = await _inventoryService.GetInventoryRequestsByDealerIdAsync(dealerId.Value);
            return View(requests);
        }

        // GET: Inventory/Create - DealerManager create new request
        [HttpGet]
        public async Task<IActionResult> Create(int? vehicleId = null)
        {
            try
            {
                _logger.LogInformation("=== Create Inventory Request Access Attempt ===");
                
                var role = GetCurrentUserRole();
                _logger.LogInformation("Current user role: '{Role}'", role);

                if (!IsDealerManager())
                {
                    _logger.LogWarning("Access denied - Role: '{Role}' is not DealerManager", role);
                    TempData["Error"] = $"Access denied. Your role '{role}' does not have permission to create inventory requests.";
                    return RedirectToAction("Login", "Account");
                }

                var dealerId = GetCurrentDealerId();
                if (dealerId == null)
                {
                    _logger.LogWarning("DealerId not found in session");
                    TempData["Error"] = "Dealer information not found. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Loading create form for DealerId: {DealerId}", dealerId.Value);

                var vehicles = await _vehicleServices.GetAllVehicle();
                ViewBag.Vehicles = vehicles.Select(v => new SelectListItem
                {
                    Value = v.VehicleId.ToString(),
                    Text = $"{v.Model} - {v.Color}",
                    Selected = vehicleId.HasValue && v.VehicleId == vehicleId.Value
                }).ToList();

                ViewBag.DealerId = dealerId;

                var model = new CreateInventoryRequest();
                if (vehicleId.HasValue)
                {
                    model.VehicleId = vehicleId.Value;
                }

                _logger.LogInformation("Create form loaded successfully");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction("Dashboard", "Dealer");
            }
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInventoryRequest request)
        {
            if (!IsDealerManager())
            {
                TempData["Error"] = "Access denied. You must be a Dealer Manager.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleServices.GetAllVehicle();
                ViewBag.Vehicles = vehicles.Select(v => new SelectListItem
                {
                    Value = v.VehicleId.ToString(),
                    Text = $"{v.Model} - {v.Color}"
                }).ToList();
                
                var dealerId = GetCurrentDealerId();
                ViewBag.DealerId = dealerId;
                return View(request);
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                TempData["Error"] = "User information not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            var result = await _inventoryService.CreateInventoryRequestAsync(request, currentUserId.Value);
            
            if (result)
            {
                TempData["Success"] = "Inventory request created successfully!";
                return RedirectToAction("MyRequests");
            }

            TempData["Error"] = "Failed to create inventory request.";
            return View(request);
        }

        // POST: Inventory/Process - Admin process request
        [HttpPost]
        public async Task<IActionResult> Process(ProcessInventoryRequestModel model)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var result = await _inventoryService.ProcessInventoryRequestAsync(model, currentUserId.Value);
            
            if (result)
            {
                return Json(new { 
                    success = true, 
                    message = model.IsApproved ? "Request approved successfully!" : "Request denied successfully!" 
                });
            }

            return Json(new { success = false, message = "Failed to process request" });
        }

        // GET: Inventory/View/{dealerId} - View dealer inventory
        public async Task<IActionResult> View(int dealerId)
        {
            var inventory = await _inventoryService.GetInventoryByDealerIdAsync(dealerId);
            ViewBag.DealerId = dealerId;
            return View(inventory);
        }

        // Helper methods
        private bool IsAdmin()
        {
            var role = GetCurrentUserRole();
            return role == "Admin";
        }

        private bool IsDealerManager()
        {
            var role = GetCurrentUserRole();
            
            // Handle empty or null role
            if (string.IsNullOrWhiteSpace(role))
            {
                _logger.LogWarning("Role is null or empty");
                return false;
            }
            
            var result = role == "DealerManager" || role.Contains("Dealer");
            _logger.LogInformation("DealerManager check - Role: '{Role}', Result: {Result}", role, result);
            return result;
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

                var tokenExpiry = jwtToken.ValidTo;
                var currentTime = DateTime.UtcNow;
                
                _logger.LogInformation("Token expiry: {Expiry}, Current time: {Now}, Valid: {IsValid}", 
                    tokenExpiry, currentTime, tokenExpiry > currentTime);
                _logger.LogInformation("ClaimTypes.Role constant value: '{ClaimType}'", ClaimTypes.Role);

                // Check token validity with proper UTC comparison
                if (tokenExpiry > currentTime)
                {
                    // Log all claims for debugging
                    _logger.LogInformation("=== ALL JWT CLAIMS ===");
                    foreach (var claim in jwtToken.Claims)
                    {
                        _logger.LogInformation("JWT Claim - Type: '{Type}', Value: '{Value}'", claim.Type, claim.Value);
                    }
                    _logger.LogInformation("=== END CLAIMS ===");

                    // Try to find role claim with multiple approaches
                    var roleClaimType1 = ClaimTypes.Role; // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    var roleClaimType2 = "role";
                    var roleClaimType3 = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

                    var roleClaim1 = jwtToken.Claims.FirstOrDefault(x => x.Type == roleClaimType1);
                    var roleClaim2 = jwtToken.Claims.FirstOrDefault(x => x.Type == roleClaimType2);
                    var roleClaim3 = jwtToken.Claims.FirstOrDefault(x => x.Type == roleClaimType3);

                    _logger.LogInformation("Role claim search results:");
                    _logger.LogInformation("  ClaimTypes.Role ('{Type}'): {Value}", roleClaimType1, roleClaim1?.Value ?? "NOT FOUND");
                    _logger.LogInformation("  'role': {Value}", roleClaim2?.Value ?? "NOT FOUND");
                    _logger.LogInformation("  Direct string: {Value}", roleClaim3?.Value ?? "NOT FOUND");

                    var role = roleClaim1?.Value ?? roleClaim2?.Value ?? roleClaim3?.Value;
                    
                    _logger.LogInformation("Final role extracted: '{Role}' (Length: {Length})", role, role?.Length ?? 0);
                    
                    // Return null if role is empty string
                    return string.IsNullOrWhiteSpace(role) ? null : role;
                }
                else
                {
                    _logger.LogWarning("Token expired - Expiry: {Expiry}, Now: {Now}", tokenExpiry, currentTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading JWT token");
            }

            return null;
        }

        private int? GetCurrentUserId()
        {
            var token = Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    // Try different claim types for UserId
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ??
                                      jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ??
                                      jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting UserId from token");
            }

            return null;
        }

        private int? GetCurrentDealerId()
        {
            var dealerId = HttpContext.Session.GetInt32("DealerId");
            _logger.LogInformation("Retrieved DealerId from session: {DealerId}", dealerId);
            return dealerId;
        }
    }
}