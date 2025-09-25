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
    public class VehicleRepository : IVehicleRepository
    {
        private readonly Prn222asm1Context _context;
        private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(Prn222asm1Context context, ILogger<VehicleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        async Task<Vehicle?> IVehicleRepository.GetDetailVehiclesAsync(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.Category)
                    .Include(v => v.VehicleInventories)
                    .Where(v => v.VehicleId == id)
                    .FirstOrDefaultAsync();

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error gettings vehicle by id {id}");
                throw;
            }
        }
    }
}
