using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;

namespace ElectricVehicleDealerManagermentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllActiveUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch
            {
                if (!await _userRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.SoftDeleteAsync(id);
            return RedirectToAction(nameof(GetAllUsers));
        }
    }
}