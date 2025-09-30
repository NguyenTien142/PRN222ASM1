using AutoMapper;

using BusinessObject.BusinessObject.UserModels.Request;
using BusinessObject.BusinessObject.UserModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Services.Intefaces;

namespace Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetUserRespond>> GetAllUsersAsync(string searchUsername = "")
        {
            var users = string.IsNullOrWhiteSpace(searchUsername)
                ? await _userRepository.GetAllUsersAsync()
                : await _userRepository.SearchByUsernameAsync(searchUsername);

            return _mapper.Map<IEnumerable<GetUserRespond>>(users);
        }

        public async Task<GetDetailUserRespond?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            return user != null ? _mapper.Map<GetDetailUserRespond>(user) : null;
        }

        public async Task<bool> EditUserAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId);
            if (user == null) return false;

            user.Role = request.Role;
            return await _userRepository.UpdateUserWithDealerAsync(user, request.DealerTypeName, request.DealerAddress);
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