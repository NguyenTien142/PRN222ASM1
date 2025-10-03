using BusinessObject.BusinessObject.VehicleModels.Request;
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
        const int ADMIN_INVENTORY_ID = 4;


        public VehicleRepository(Prn222asm1Context context, ILogger<VehicleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Vehicle?> GetDetailVehiclesAsync(int id)
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

        public async Task<List<Vehicle>> GetVehicleBuyDealerIdAsync(int dealerId)
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .Include(v => v.Category)
                    .Include(v => v.VehicleInventories)
                        .ThenInclude(vi => vi.Inventory)
                            .ThenInclude(i => i.Dealer)
                                .ThenInclude(d => d.DealerType)
                    .Where(v => v.VehicleInventories.Any(vi => vi.Inventory.DealerId == dealerId))
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting vehicles for dealer id {dealerId}");
                throw;
            }
        }

        public async Task<List<VehicleInventory>> GetVehiclesByInventoryIdAsync(int inventoryId)
        {
           
                try
                {
                    _logger.LogInformation("Getting VehicleInventories for inventoryId: {InventoryId}", inventoryId);

                    var vehicleInventories = await _context.VehicleInventories
                        .Where(vi => vi.InventoryId == inventoryId)
                        .Include(vi => vi.Vehicle)
                            .ThenInclude(v => v.Category)
                        .ToListAsync();

                    _logger.LogInformation("Retrieved {Count} vehicle inventories for inventory {InventoryId}",
                        vehicleInventories.Count, inventoryId);

                    return vehicleInventories;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting vehicle inventories for inventory id {InventoryId}", inventoryId);
                    throw;
                }
            }

        public async Task UpdateVehicleInventoryAsync(UpdateVehicleInventoryRequest request)
        {
            try
            {
                _logger.LogInformation("Updating vehicle and inventory for VehicleId: {VehicleId}", request.VehicleId);

                // 1. Update Vehicle information
                var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
                if (vehicle == null)
                {
                    throw new InvalidOperationException($"Vehicle with ID {request.VehicleId} not found");
                }

                // Update vehicle properties
                vehicle.Model = request.Model;
                vehicle.Color = request.Color;
                vehicle.Price = request.Price;
                vehicle.Version = request.Version;
                vehicle.Image = request.Image;

                _context.Vehicles.Update(vehicle);

                // 2. Update VehicleInventory quantity (assuming admin inventory ID = 2)

                var vehicleInventory = await _context.VehicleInventories
                    .FirstOrDefaultAsync(vi => vi.VehicleId == request.VehicleId && vi.InventoryId == ADMIN_INVENTORY_ID);

                if (vehicleInventory != null)
                {
                    // Update existing inventory
                    vehicleInventory.Quantity = request.Quantity;
                    _context.VehicleInventories.Update(vehicleInventory);
                }
                else
                {
                    // Create new inventory record if not exists
                    throw new InvalidOperationException($"Inventory with ID {request.VehicleId} not found");

                }

                // 3. Save all changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated vehicle {VehicleId} and inventory with quantity {Quantity}",
                    request.VehicleId, request.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle inventory for VehicleId: {VehicleId}", request.VehicleId);
                throw;
            }
        }

        public async Task DeleteVehicleInventoryAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting vehicle inventory for VehicleId: {VehicleId}", id);

                // 1. Find the vehicle inventory record in admin inventory (ID = 2)
                var vehicleInventory = await _context.VehicleInventories
                    .FirstOrDefaultAsync(vi => vi.VehicleId == id && vi.InventoryId == ADMIN_INVENTORY_ID);

                if (vehicleInventory == null)
                {
                    throw new InvalidOperationException($"Vehicle inventory with VehicleId {id} not found in admin inventory");
                }
                

                // 4. Remove the vehicle inventory record
                _context.VehicleInventories.Remove(vehicleInventory);

                // 5. Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted vehicle inventory for VehicleId: {VehicleId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle inventory for VehicleId: {VehicleId}", id);
                throw;
            }
        }

        public async Task<int> CreateVehicleAsync(CreateVehicleRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new vehicle: {Model} - {Color}", request.Model, request.Color);

                var vehicle = new Vehicle
                {
                    CategoryId = request.CategoryId,
                    Color = request.Color,
                    Price = request.Price,
                    ManufactureDate = request.ManufactureDate,
                    Model = request.Model,
                    Version = request.Version,
                    Image = request.Image
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created vehicle with ID: {VehicleId}", vehicle.VehicleId);
                return vehicle.VehicleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle: {Model} - {Color}", request.Model, request.Color);
                throw;
            }
        }

        public async Task CreateVehicleInventoryAsync(CreateVehicleInventoryRequest request)
        {
            try
            {
                _logger.LogInformation("Creating vehicle inventory: VehicleId={VehicleId}, InventoryId={InventoryId}, Quantity={Quantity}", 
                    request.VehicleId, request.InventoryId, request.Quantity);

                var vehicleInventory = new VehicleInventory
                {
                    VehicleId = request.VehicleId,
                    InventoryId = request.InventoryId,
                    Quantity = request.Quantity
                };

                _context.VehicleInventories.Add(vehicleInventory);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created vehicle inventory for VehicleId: {VehicleId}", request.VehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle inventory for VehicleId: {VehicleId}", request.VehicleId);
                throw;
            }
        }

        public async Task<bool> VehicleExistsAsync(string model, string color, string version)
        {
            try
            {
                return await _context.Vehicles
                    .AnyAsync(v => v.Model.ToLower() == model.ToLower() 
                                && v.Color.ToLower() == color.ToLower() 
                                && (v.Version ?? "").ToLower() == (version ?? "").ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if vehicle exists: {Model} - {Color} - {Version}", model, color, version);
                throw;
            }
        }
    }
}
