using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>?> GetAllCustomers();
    }
}
