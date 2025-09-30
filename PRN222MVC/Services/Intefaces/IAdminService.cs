using Repositories.Model;

namespace Services.Intefaces
{
    public interface IAdminService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(string searchUsername = "");
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> EditUserAsync(int id, string newRole);
        Task<bool> DeleteUserAsync(int id);
    }
}