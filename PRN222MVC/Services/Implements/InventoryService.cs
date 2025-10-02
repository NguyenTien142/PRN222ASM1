using AutoMapper;
using BusinessObject.BusinessObject.InventoryModels.Request;
using BusinessObject.BusinessObject.InventoryModels.Respond;
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
                var request = await _inventoryRequestRepository.GetByIdAsync(model.RequestId);
                if (request == null) return false;

                request.Status = model.IsApproved ? "Approved" : "Denied";
                request.ProcessedDate = DateTime.Now;
                request.ProcessedBy = processedByUserId;
                request.AdminComment = model.AdminComment;

                await _inventoryRequestRepository.UpdateAsync(request);
                return true;
            }
            catch
            {
                return false;
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