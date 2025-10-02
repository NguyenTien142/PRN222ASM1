using Repositories.CustomRepositories.Interfaces;
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

    }
}
