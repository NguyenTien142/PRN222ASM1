using BusinessObject.BusinessObject.OrderModels.Response;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Intefaces
{
    public interface IOrderService
    {
        Task<List<GetSuccessfulOrderResponse>> GetSuccessfulOrderAsync(int userId);
        Task<List<GetPendingOrderResponse>> GetPendingOrderAsync(int userId);
        Task<decimal> GetTotalEarningsByUserAsync(int userId);

    }
}
