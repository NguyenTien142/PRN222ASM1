using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAndPasswordAsync(string username, string hashedPassword);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<User?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task SoftDeleteAsync(int id);
        Task<IEnumerable<User>> SearchByUsernameAsync(string username);
    }
}
