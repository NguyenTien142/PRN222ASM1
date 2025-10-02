using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> IsVehicleBookedAsync(int vehicleId, DateTime start, DateTime end);
        Task<bool> CheckVehicleConflictAsync(int vehicleId, DateTime appointmentDate);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
    }
}
