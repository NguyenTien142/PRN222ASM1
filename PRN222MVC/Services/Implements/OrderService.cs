using AutoMapper;
using BusinessObject.BusinessObject.OrderModels.Response;
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
    public class OrderService : IOrderService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetPendingOrderResponse>> GetPendingOrderAsync(int userId)
        {
            var orderRepo = _unitOfWork.GetCustomRepository<IOrderRepository>();
            var orders = await orderRepo.GetPendingOrderAsync(userId);
            var orderResponses = _mapper.Map<List<GetPendingOrderResponse>>(orders);
            return orderResponses;
        }

        public async Task<List<GetSuccessfulOrderResponse>> GetSuccessfulOrderAsync(int userId)
        {
            var orderRepo = _unitOfWork.GetCustomRepository<IOrderRepository>();
            var orders = await orderRepo.GetSuccessfulOrderAsync(userId);
            var orderResponses = _mapper.Map<List<GetSuccessfulOrderResponse>>(orders);
            return orderResponses;
        }

        public async Task<decimal> GetTotalEarningsByUserAsync(int userId)
        {
            var orderRepo = _unitOfWork.GetCustomRepository<IOrderRepository>();
            var orders = await orderRepo.GetTotalEarningsByUserAsync(userId);
            return orders;
        }
    }
}
