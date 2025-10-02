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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(Prn222asm1Context context) : base(context)
        {
        }

        public async Task<List<Customer>?> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return customers;
        }
    }
}
