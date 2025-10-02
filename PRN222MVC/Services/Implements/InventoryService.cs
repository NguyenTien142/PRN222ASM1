using AutoMapper;
using Repositories.CustomRepositories.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<int> GetTotalStockQuantityByDealerAsync(int dealerId)
        {
            var inventoryRepo = _unitOfWork.GetCustomRepository<IInventoryRepository>();
            var quantity = inventoryRepo.GetTotalStockQuantityByDealerAsync(dealerId);
            return quantity;
        }
    }
}
