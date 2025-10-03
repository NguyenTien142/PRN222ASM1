using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using System;
using System.Linq;
using System.Text;

namespace Repositories.CustomRepositories.Implements
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(Prn222asm1Context context) : base(context)
        {
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            var order = await _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.OrderVehicles)
                .ThenInclude(ov => ov.Vehicle)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
            return order;
        }

        public async Task<Order?> GetOrderById(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
            return order;
        }

        public async Task<List<Order>?> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(x => x.Customer)
                .Where(x => x.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetSuccessfulOrderAsync(int userId)
        {
            return await GetSuccessfulOrderAsync(userId, null, null);
        }

        public async Task<List<Order>> GetSuccessfulOrderAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderVehicles)
                .ThenInclude(ov => ov.Vehicle)
                .Where(o => o.UserId == userId && o.Status == "Approved");

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Order>> GetPendingOrderAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Pending").ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetAllPendingOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Where(o => o.Status == "Pending")
                .ToListAsync();
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalEarningsByUserAsync(int userId)
        {
            return await GetTotalEarningsByUserAsync(userId, null, null);
        }

        public async Task<decimal> GetTotalEarningsByUserAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            var successfulOrders = await GetSuccessfulOrderAsync(userId, startDate, endDate);

            // Tính tổng TotalAmount trực tiếp thay vì dùng phương thức riêng
            var totalEarnings = successfulOrders.Sum(order => order.TotalAmount);

            // Nếu không có orders nào, trả về 0
            return totalEarnings > 0 ? totalEarnings : 0;
        }
    }
}
