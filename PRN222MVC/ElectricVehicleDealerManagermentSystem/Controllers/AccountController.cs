using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Intefaces;
using BusinessObject.BusinessObject.UserModels.Request;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                if (!result.IsSuccess)
                {
                    TempData["Error"] = result.Message;
                    return View(request);
                }

                // Debug logging
                _logger.LogInformation("Login successful for user: {Username}, Role: {Role}", 
                    result.User?.Username, result.User?.Role);

                // Store token in cookie - Fixed for HTTP development
                Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps, // Only secure in HTTPS
                    SameSite = SameSiteMode.Lax, // Changed from Strict to Lax
                    Expires = DateTime.Now.AddHours(3),
                    Path = "/" // Ensure cookie is available for entire site
                });

                if (result.User != null)
                {
                    HttpContext.Session.SetInt32("UserId", result.User.UserId);
                }

                if (result.User != null && result.User.DealerId > 0)
                {
                    HttpContext.Session.SetInt32("DealerId", result.User.DealerId);
                    _logger.LogInformation("DealerId set in session: {DealerId}", result.User.DealerId);
                }
                
                var userRole = result.User?.Role ?? "Unknown";
                _logger.LogInformation("Determining redirect for role: {Role}", userRole);

                // Role-based redirects
                switch (userRole)
                {
                    case "Admin":
                        _logger.LogInformation("Redirecting to Admin Dashboard");
                        return RedirectToAction("Dashboard", "Admin");
                    
                    case "DealerManager":
                        _logger.LogInformation("Redirecting to Dealer Dashboard");
                        return RedirectToAction("Dashboard", "Dealer");
                    
                    case "DealerStaff":
                        _logger.LogInformation("Redirecting to Home (DealerStaff)");
                        return RedirectToAction("Index", "Staff");
                    
                    default:
                        _logger.LogInformation("Redirecting to Home (default/unknown role)");
                        return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login for user: {Username}", request.Username);
                TempData["Error"] = "An error occurred during login. Please try again.";
                return View(request);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    TempData["Error"] = "Username and password are required";
                    return View(request);
                }

                var result = await _authService.RegisterAsync(request);

                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Login");
                }

                TempData["Error"] = result.Message;
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register: {Message}", ex.Message);
                TempData["Error"] = "An error occurred during registration. Please try again.";
                return View(request);
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}