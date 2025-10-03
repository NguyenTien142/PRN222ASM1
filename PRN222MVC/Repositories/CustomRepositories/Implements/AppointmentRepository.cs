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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly Prn222asm1Context _context;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(Prn222asm1Context context, ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CheckVehicleConflictAsync(int vehicleId, DateTime appointmentDate)
        {
            return await _context.Appointments
                .AnyAsync(a => a.VehicleId == vehicleId && a.AppointmentDate == appointmentDate);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> IsVehicleBookedAsync(int vehicleId, DateTime start, DateTime end)
        {
            return await _context.Appointments
                .AnyAsync(a => a.VehicleId == vehicleId &&
                    (
                        (start >= a.AppointmentDate && start < a.AppointmentDate.AddHours(1))  // starts inside
                        || (end > a.AppointmentDate && end <= a.AppointmentDate.AddHours(1)) // ends inside
                        || (start <= a.AppointmentDate && end >= a.AppointmentDate.AddHours(1)) // fully overlaps
                    ));
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Vehicle)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }
    }
}
