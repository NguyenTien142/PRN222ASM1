using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BusinessObject.BusinessObject.UserModels.Request;
using BusinessObject.BusinessObject.UserModels.Respond;
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
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, JwtService jwtService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<LoginRespond> LoginAsync(LoginRequest request)
        {
            var hashedPassword = HashPassword(request.Password);
            var user = await _userRepository.GetByUsernameAndPasswordAsync(request.Username, hashedPassword);

            if (user == null)
            {
                return new LoginRespond
                {
                    IsSuccess = false,
                    Message = "Invalid username or password"
                };
            }

            var token = _jwtService.GenerateToken(user);
            var userRespond = _mapper.Map<GetUserRespond>(user);

            return new LoginRespond
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = token,
                User = userRespond
            };
        }

        public async Task<RegisterRespond> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                return new RegisterRespond
                {
                    IsSuccess = false,
                    Message = "Username already exists"
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new RegisterRespond
                {
                    IsSuccess = false,
                    Message = "Passwords do not match"
                };
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = HashPassword(request.Password);
            user.Role = "DealerStaff"; // Default role

            var result = await _userRepository.AddAsync(user);

            return new RegisterRespond
            {
                IsSuccess = result,
                Message = result ? "Registration successful" : "Registration failed"
            };
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