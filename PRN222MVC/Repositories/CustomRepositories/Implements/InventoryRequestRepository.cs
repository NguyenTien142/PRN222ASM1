using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Implements;

namespace Repositories.CustomRepositories.Implements
{
    public class InventoryRequestRepository : GenericRepository<InventoryRequest>, IInventoryRequestRepository
    {
        public InventoryRequestRepository(Prn222asm1Context context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryRequest>> GetByDealerIdAsync(int dealerId)
        {
            return await _context.InventoryRequests
                .Include(ir => ir.Vehicle)
                .Include(ir => ir.RequestedByUser)
                .Include(ir => ir.ProcessedByUser)
                .Where(ir => ir.DealerId == dealerId)
                .OrderByDescending(ir => ir.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryRequest>> GetByStatusAsync(string status)
        {
            return await _context.InventoryRequests
                .Include(ir => ir.Vehicle)
                .Include(ir => ir.Dealer)
                .Include(ir => ir.RequestedByUser)
                .Include(ir => ir.ProcessedByUser)
                .Where(ir => ir.Status == status)
                .OrderByDescending(ir => ir.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryRequest>> GetAllWithDetailsAsync()
        {
            return await _context.InventoryRequests
                .Include(ir => ir.Vehicle)
                .Include(ir => ir.Dealer)
                .Include(ir => ir.RequestedByUser)
                .Include(ir => ir.ProcessedByUser)
                .OrderByDescending(ir => ir.RequestDate)
                .ToListAsync();
        }

        public async Task<InventoryRequest?> GetByIdWithDetailsAsync(int requestId)
        {
            return await _context.InventoryRequests
                .Include(ir => ir.Vehicle)
                .Include(ir => ir.Dealer)
                .Include(ir => ir.RequestedByUser)
                .Include(ir => ir.ProcessedByUser)
                .FirstOrDefaultAsync(ir => ir.RequestId == requestId);
        }
    }
}