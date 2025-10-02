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

                // Store token in cookie
                Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(3)
                });

                if (result.User != null && result.User.DealerId > 0)
                {
                    HttpContext.Session.SetInt32("DealerId", result.User.DealerId);
                }

                if (result.User?.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
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