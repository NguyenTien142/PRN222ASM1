using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Microsoft.Extensions.Logging;

namespace Repositories.CustomRepositories.Implements
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(Prn222asm1Context context, ILogger<UserRepository> logger = null) : base(context)
        {
            _logger = logger;
        }

        public async Task<User?> GetByUsernameAndPasswordAsync(string username, string hashedPassword)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                        .ThenInclude(d => d.DealerType)
                    .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetByUsernameAndPasswordAsync");
                throw;
            }
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in ExistsByUsernameAsync");
                throw;
            }
        }

        public async Task<User?> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                        .ThenInclude(d => d.DealerType)
                    .FirstOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetByIdWithDetailsAsync");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                        .ThenInclude(d => d.DealerType)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetAllUsersAsync");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                        .ThenInclude(d => d.DealerType)
                    .Where(u => u.Role != "Inactive")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetAllActiveUsersAsync");
                throw;
            }
        }

        public async Task<IEnumerable<User>> SearchByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                        .ThenInclude(d => d.DealerType)
                    .Where(u => u.Username.Contains(username))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in SearchByUsernameAsync");
                throw;
            }
        }

        public async Task SoftDeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.Role = "Inactive";
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in SoftDeleteAsync");
                throw;
            }
        }

        // Override AddAsync to handle DealerId
        public override async Task<bool> AddAsync(User entity)
        {
            try
            {
                // Get the first available dealer or create a default one
                var dealer = await _context.Dealers.FirstOrDefaultAsync();
                if (dealer == null)
                {
                    // Create a default dealer type if none exists
                    var dealerType = await _context.DealerTypes.FirstOrDefaultAsync() 
                        ?? await CreateDefaultDealerType();

                    // Create a default dealer
                    dealer = new Dealer
                    {
                        DealerTypeId = dealerType.DealerTypeId,
                        Address = "Default Address"
                    };
                    _context.Dealers.Add(dealer);
                    await _context.SaveChangesAsync();
                }

                // Set the DealerId
                entity.DealerId = dealer.DealerId;

                // Add the user
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in AddAsync: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger?.LogError(ex.InnerException, "Inner Exception: {Message}", ex.InnerException.Message);
                }
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(User entity)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(entity.UserId);
                if (existingUser == null)
                    return false;

                // Update only allowed fields
                existingUser.Role = entity.Role;

                // Mark as modified
                _context.Entry(existingUser).State = EntityState.Modified;
                
                // Save changes
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in UpdateAsync: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger?.LogError(ex.InnerException, "Inner Exception: {Message}", ex.InnerException.Message);
                }
                return false;
            }
        }

        private async Task<DealerType> CreateDefaultDealerType()
        {
            try
            {
                var dealerType = new DealerType
                {
                    TypeName = "Staff"
                };
                _context.DealerTypes.Add(dealerType);
                await _context.SaveChangesAsync();
                return dealerType;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in CreateDefaultDealerType");
                throw;
            }
        }
    }
}
