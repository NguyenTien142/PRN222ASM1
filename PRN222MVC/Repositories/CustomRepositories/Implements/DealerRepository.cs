using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Implements
{
    public class DealerRepository : IDealerRepository
    {
        private readonly Prn222asm1Context _context;
        private readonly ILogger<DealerRepository> _logger;

        public DealerRepository(Prn222asm1Context context, ILogger<DealerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Dealer?> GetDealerByIdAsync(int dealerId)
        {
            try
            {
                var dealer = await _context.Dealers
                    .Include(d => d.DealerType)
                    .Where(d => d.DealerId == dealerId)
                    .FirstOrDefaultAsync();
                return dealer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting dealer by id {dealerId}");
                throw;
            }
        }
    }
}
