using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Services.Intefaces;

namespace Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string searchUsername = "")
        {
            if (string.IsNullOrWhiteSpace(searchUsername))
                return await _userRepository.GetAllUsersAsync();
            return await _userRepository.SearchByUsernameAsync(searchUsername);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<bool> EditUserAsync(int id, string newRole, string dealerTypeName, string dealerAddress)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            if (user == null) return false;
            
            user.Role = newRole;
            return await _userRepository.UpdateUserWithDealerAsync(user, dealerTypeName, dealerAddress);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.SoftDeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}