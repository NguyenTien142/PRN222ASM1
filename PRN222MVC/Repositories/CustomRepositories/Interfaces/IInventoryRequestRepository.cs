using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IInventoryRequestRepository : IGenericRepository<InventoryRequest>
    {
        Task<IEnumerable<InventoryRequest>> GetByDealerIdAsync(int dealerId);
        Task<IEnumerable<InventoryRequest>> GetByStatusAsync(string status);
        Task<IEnumerable<InventoryRequest>> GetAllWithDetailsAsync();
        Task<InventoryRequest?> GetByIdWithDetailsAsync(int requestId);
    }
}