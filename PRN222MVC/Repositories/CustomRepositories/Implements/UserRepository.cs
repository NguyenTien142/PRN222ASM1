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

        // Override AddAsync to handle DealerId with default DealerStaff role and Agent DealerType
        public override async Task<bool> AddAsync(User entity)
        {
            try
            {
                // Set default role to DealerStaff
                entity.Role = "DealerStaff";

                // Get or create Agent DealerType
                var dealerType = await _context.DealerTypes.FirstOrDefaultAsync(dt => dt.TypeName == "Agent")
                    ?? await CreateDealerType("Agent");

                // Create new Dealer with Agent type
                var dealer = new Dealer
                {
                    DealerTypeId = dealerType.DealerTypeId,
                    Address = "Default Address"
                };
                _context.Dealers.Add(dealer);
                await _context.SaveChangesAsync();

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

                existingUser.Role = entity.Role;
                _context.Entry(existingUser).State = EntityState.Modified;
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

        public async Task<bool> UpdateUserWithDealerAsync(User entity, string dealerTypeName, string dealerAddress)
        {
            try
            {
                var existingUser = await _context.Users
                    .Include(u => u.Dealer)
                    .ThenInclude(d => d.DealerType)
                    .FirstOrDefaultAsync(u => u.UserId == entity.UserId);

                if (existingUser == null)
                    return false;

                // Validate dealer type name
                if (!IsValidDealerType(dealerTypeName))
                {
                    _logger?.LogError("Invalid dealer type: {DealerType}", dealerTypeName);
                    return false;
                }

                // Update user role
                existingUser.Role = entity.Role;

                // Update dealer type and address if provided
                if (existingUser.Dealer != null && !string.IsNullOrEmpty(dealerTypeName))
                {
                    // Get or create dealer type
                    var dealerType = await _context.DealerTypes
                        .FirstOrDefaultAsync(dt => dt.TypeName == dealerTypeName);

                    if (dealerType == null)
                    {
                        dealerType = await CreateDealerType(dealerTypeName);
                    }

                    existingUser.Dealer.DealerTypeId = dealerType.DealerTypeId;

                    if (!string.IsNullOrEmpty(dealerAddress))
                    {
                        existingUser.Dealer.Address = dealerAddress;
                    }
                }

                _context.Entry(existingUser).State = EntityState.Modified;
                if (existingUser.Dealer != null)
                {
                    _context.Entry(existingUser.Dealer).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in UpdateUserWithDealerAsync: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger?.LogError(ex.InnerException, "Inner Exception: {Message}", ex.InnerException.Message);
                }
                return false;
            }
        }

        private bool IsValidDealerType(string typeName)
        {
            return typeName == "Headquarter" || typeName == "Agent";
        }

        private async Task<DealerType> CreateDealerType(string typeName)
        {
            try
            {
                var dealerType = new DealerType
                {
                    TypeName = typeName
                };
                _context.DealerTypes.Add(dealerType);
                await _context.SaveChangesAsync();
                return dealerType;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in CreateDealerType: {Message}", ex.Message);
                throw;
            }
        }
    }
}
