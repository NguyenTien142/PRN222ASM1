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
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Model;

namespace Repositories.CustomRepositories.Implements
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(Prn222asm1Context context) : base(context)
        {
        }

        public async Task<List<Order>?> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(x => x.Customer)
                .Where(x => x.UserId == userId).ToListAsync();
            return orders;
        }

        public Task<Order?> GetOrderById(int userId, int orderId)
        {
            var order = _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.OrderVehicles)
                .ThenInclude(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
            return order;
        }


        public async Task<List<Order>> GetPendingOrderAsync(int userId)
        {
            var dealerStaff = await _context.Users.FirstOrDefaultAsync(o => o.DealerId == userId && o.Role == "DealerStaff");
            var orders = await _context.Orders
                .Where(o => o.UserId == dealerStaff.UserId && o.Status == "Pending").ToListAsync();
            return  orders;
        }

        public async Task<List<Order>> GetSuccessfulOrderAsync(int userId)
        {
            var dealerStaff = await _context.Users.FirstOrDefaultAsync(o => o.DealerId == userId && o.Role == "DealerStaff");
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.UserId == dealerStaff.UserId && o.Status == "Payed").ToListAsync();
            return orders;
        }

        public async Task<decimal> GetTotalEarningsByUserAsync(int userId)
        {
            var dealerStaff = await _context.Users.FirstOrDefaultAsync(o => o.DealerId == userId && o.Role == "DealerStaff");
            var successfulOrders = await GetSuccessfulOrderAsync(dealerStaff.UserId);
            return successfulOrders.Sum(o => o.TotalAmount);
        }
    }
}
