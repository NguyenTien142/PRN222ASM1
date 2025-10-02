using BusinessObject.BusinessObject.InventoryModels.Request;
using BusinessObject.BusinessObject.InventoryModels.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<int> GetTotalStockQuantityByDealerAsync(int dealerId);
    }
}