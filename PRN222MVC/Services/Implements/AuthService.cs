using System.Security.Cryptography;
using System.Text;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Services.Intefaces;
using Services.Services;

namespace Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthService(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<(User? user, string? token)> LoginAsync(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var user = await _userRepository.GetByUsernameAndPasswordAsync(username, hashedPassword);

            if (user == null)
            {
                return (null, null);
            }

            var token = _jwtService.GenerateToken(user);
            return (user, token);
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            if (await _userRepository.ExistsByUsernameAsync(user.Username))
            {
                return false;
            }

            user.PasswordHash = HashPassword(password);
            user.Role = "User"; // Default role

            await _userRepository.AddAsync(user);
            return true;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}