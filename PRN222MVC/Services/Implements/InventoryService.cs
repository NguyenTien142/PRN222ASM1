using AutoMapper;
using BusinessObject.BusinessObject.InventoryModels.Request;
using BusinessObject.BusinessObject.InventoryModels.Respond;
using BusinessObject.BusinessObject.VehicleModels.Request;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRequestRepository _inventoryRequestRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(
            IInventoryRequestRepository inventoryRequestRepository, 
            IVehicleRepository vehicleRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _inventoryRequestRepository = inventoryRequestRepository;
            _vehicleRepository = vehicleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<bool> CreateInventoryRequestAsync(CreateInventoryRequest request, int requestedByUserId)
        {
            try
            {
                var inventoryRequest = new InventoryRequest
                {
                    VehicleId = request.VehicleId,
                    DealerId = request.DealerId,
                    RequestedBy = requestedByUserId,
                    RequestedQuantity = request.Quantity,
                    Reason = request.Reason,
                    Status = "Pending",
                    RequestDate = DateTime.Now
                };

                await _inventoryRequestRepository.AddAsync(inventoryRequest);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<GetInventoryRequestRespond>> GetAllInventoryRequestsAsync()
        {
            var requests = await _inventoryRequestRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<GetInventoryRequestRespond>>(requests);
        }

        public async Task<IEnumerable<GetInventoryRequestRespond>> GetInventoryRequestsByDealerIdAsync(int dealerId)
        {
            var requests = await _inventoryRequestRepository.GetByDealerIdAsync(dealerId);
            return _mapper.Map<IEnumerable<GetInventoryRequestRespond>>(requests);
        }

        public async Task<GetInventoryRequestRespond?> GetInventoryRequestByIdAsync(int requestId)
        {
            var request = await _inventoryRequestRepository.GetByIdWithDetailsAsync(requestId);
            return request != null ? _mapper.Map<GetInventoryRequestRespond>(request) : null;
        }

        public async Task<bool> ProcessInventoryRequestAsync(ProcessInventoryRequestModel model, int processedByUserId)
        {
            try
            {
                var request = await _inventoryRequestRepository.GetByIdWithDetailsAsync(model.RequestId);
                if (request == null) return false;

                // Update request status
                request.Status = model.IsApproved ? "Approved" : "Denied";
                request.ProcessedDate = DateTime.Now;
                request.ProcessedBy = processedByUserId;
                request.AdminComment = model.AdminComment;

                await _inventoryRequestRepository.UpdateAsync(request);

                // ✅ NEW: If approved, add quantity to dealer's inventory
                if (model.IsApproved)
                {
                    await AddQuantityToDealerInventoryAsync(request.VehicleId, request.DealerId, request.RequestedQuantity);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thêm quantity vào VehicleInventory của dealer khi request được approve
        /// </summary>
        private async Task AddQuantityToDealerInventoryAsync(int vehicleId, int dealerId, int quantity)
        {
            try
            {
                // Get dealer's inventory
                var inventoryRepo = _unitOfWork.GetRepository<Inventory>();
                var dealerInventory = (await inventoryRepo.GetAllAsync())
                    .FirstOrDefault(i => i.DealerId == dealerId);

                if (dealerInventory == null)
                {
                    throw new InvalidOperationException($"Inventory not found for dealer {dealerId}");
                }

                // Check if VehicleInventory record already exists
                var vehicleInventoryRepo = _unitOfWork.GetRepository<VehicleInventory>();
                var existingVehicleInventory = (await vehicleInventoryRepo.GetAllAsync())
                    .FirstOrDefault(vi => vi.VehicleId == vehicleId && vi.InventoryId == dealerInventory.InventoryId);

                if (existingVehicleInventory != null)
                {
                    // Update existing quantity
                    existingVehicleInventory.Quantity += quantity;
                    vehicleInventoryRepo.Update(existingVehicleInventory);
                }
                else
                {
                    // Create new VehicleInventory record
                    var newVehicleInventory = new VehicleInventory
                    {
                        VehicleId = vehicleId,
                        InventoryId = dealerInventory.InventoryId,
                        Quantity = quantity
                    };
                    await vehicleInventoryRepo.AddAsync(newVehicleInventory);
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to add quantity to dealer inventory: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<object>> GetInventoryByDealerIdAsync(int dealerId)
        {
            // This would typically return actual inventory data
            // For now, return empty list as placeholder
            return new List<object>();
        }

        public async Task<Dictionary<string, int>> GetInventoryRequestStatsAsync(int dealerId)
        {
            var requests = await _inventoryRequestRepository.GetByDealerIdAsync(dealerId);

            return new Dictionary<string, int>
            {
                ["Total"] = requests.Count(),
                ["Pending"] = requests.Count(r => r.Status == "Pending"),
                ["Approved"] = requests.Count(r => r.Status == "Approved"),
                ["Denied"] = requests.Count(r => r.Status == "Denied")
            };
        }

        public Task<int> GetTotalStockQuantityByDealerAsync(int dealerId)
        {
            var inventoryRepo = _unitOfWork.GetCustomRepository<IInventoryRepository>();
            var quantity = inventoryRepo.GetTotalStockQuantityByDealerAsync(dealerId);
            return quantity;
        }
    }
}