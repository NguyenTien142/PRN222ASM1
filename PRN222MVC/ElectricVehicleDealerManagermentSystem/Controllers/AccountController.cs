using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Model;
using Services.Intefaces;

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
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var (user, token) = await _authService.LoginAsync(username, password);

                if (user == null)
                {
                    TempData["Error"] = "Invalid username or password";
                    return View();
                }

                // Store token in cookie
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(3)
                });

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                TempData["Error"] = "An error occurred during login. Please try again.";
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Username and password are required";
                    return View();
                }

                if (await _authService.RegisterAsync(user, password))
                {
                    TempData["Success"] = "Registration successful. Please login.";
                    return RedirectToAction("Login");
                }

                TempData["Error"] = "Username already exists";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner Exception: {Message}", ex.InnerException.Message);
                }
                TempData["Error"] = "An error occurred during registration. Please try again.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token");
            return RedirectToAction("Index", "Home");
        }
    }
}