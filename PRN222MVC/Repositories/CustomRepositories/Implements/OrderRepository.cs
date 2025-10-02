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
    public class OrderRepository : IOrderRepository
    {

        private readonly Prn222asm1Context _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(Prn222asm1Context context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Order>> GetPendingOrderAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Pending").ToListAsync();
            return  orders;
        }

        public async Task<List<Order>> GetSuccessfulOrderAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.UserId == userId && o.Status == "Done").ToListAsync();
            return orders;
        }

        public async Task<decimal> GetTotalEarningsByUserAsync(int userId)
        {
            var successfulOrders = await GetSuccessfulOrderAsync(userId);
            return successfulOrders.Sum(o => o.TotalAmount);
        }
    }
}
