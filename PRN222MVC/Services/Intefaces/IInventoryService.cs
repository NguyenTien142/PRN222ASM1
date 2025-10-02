using BusinessObject.BusinessObject.InventoryModels.Request;
using BusinessObject.BusinessObject.InventoryModels.Respond;

namespace Services.Intefaces
{
    public interface IInventoryService
    {
        Task<bool> CreateInventoryRequestAsync(CreateInventoryRequest request, int requestedByUserId);
        Task<IEnumerable<GetInventoryRequestRespond>> GetAllInventoryRequestsAsync();
        Task<IEnumerable<GetInventoryRequestRespond>> GetInventoryRequestsByDealerIdAsync(int dealerId);
        Task<GetInventoryRequestRespond?> GetInventoryRequestByIdAsync(int requestId);
        Task<bool> ProcessInventoryRequestAsync(ProcessInventoryRequestModel model, int processedByUserId);
        Task<IEnumerable<object>> GetInventoryByDealerIdAsync(int dealerId);
        Task<Dictionary<string, int>> GetInventoryRequestStatsAsync(int dealerId);
    }
}