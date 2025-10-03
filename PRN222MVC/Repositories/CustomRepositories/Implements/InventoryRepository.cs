using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Implements
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly Prn222asm1Context _context;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(Prn222asm1Context context, ILogger<InventoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<int> GetTotalStockQuantityByDealerAsync(int dealerId)
        {
            try
            {
                var totalStockQuantity = _context.Inventories
                    .Where(inv => inv.DealerId == dealerId)
                    .SelectMany(inv => inv.VehicleInventories)
                    .SumAsync(i => i.Quantity);
                return totalStockQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting total stock quantity for dealer id {dealerId}");
                throw;
            }
        }
    }
}
