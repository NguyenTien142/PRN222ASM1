using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>?> GetOrdersByUserId(int userId);
        Task<Order?> GetOrderById(int userId, int orderId);
        Task<List<Order>> GetSuccessfulOrderAsync(int userId);
        Task<List<Order>> GetPendingOrderAsync(int userId);
        Task<decimal> GetTotalEarningsByUserAsync(int userId);

    }
}
