using AutoMapper;
using BusinessObject.BusinessObject.ReportModels;
using BusinessObject.BusinessObject.UserModels.Request;
using BusinessObject.BusinessObject.UserModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUserRepository userRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetUserRespond>> GetAllUsersAsync(string searchUsername = "")
        {
            var users = string.IsNullOrWhiteSpace(searchUsername)
                ? await _userRepository.GetAllUsersAsync()
                : await _userRepository.SearchByUsernameAsync(searchUsername);

            return _mapper.Map<IEnumerable<GetUserRespond>>(users);
        }

        public async Task<GetDetailUserRespond?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(id);
            return user != null ? _mapper.Map<GetDetailUserRespond>(user) : null;
        }

        public async Task<bool> EditUserAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId);
            if (user == null) return false;

            user.Role = request.Role;
            return await _userRepository.UpdateUserWithDealerAsync(user, request.DealerTypeName, request.DealerAddress);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.SoftDeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<DealerSalesReportResponse>> GetDealerSalesReportAsync()
        {
            return await GetDealerSalesReportAsync(null, null);
        }

        public async Task<IEnumerable<DealerSalesReportResponse>> GetDealerSalesReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            var dealers = allUsers.Where(u => u.Role == "DealerStaff" && u.Dealer != null);
            var reports = new List<DealerSalesReportResponse>();

            foreach (var dealer in dealers)
            {
                var successfulOrders = await _orderRepository.GetSuccessfulOrderAsync(dealer.UserId, startDate, endDate);
                var pendingOrders = await _orderRepository.GetPendingOrderAsync(dealer.UserId);

                // Tính tổng earnings từ tất cả orders thành công
                var totalEarnings = successfulOrders.Sum(order => order.TotalAmount);

                // Tính tổng vehicles sold từ tất cả orders thành công
                var totalVehiclesSold = successfulOrders.Sum(order =>
                    order.OrderVehicles?.Sum(ov => ov.Quantity) ?? 0);

                // Đảm bảo totalVehiclesSold không âm
                totalVehiclesSold = Math.Max(0, totalVehiclesSold);

                var report = new DealerSalesReportResponse
                {
                    DealerName = dealer.Username,
                    DealerType = dealer.Dealer?.DealerType?.TypeName ?? "N/A",
                    DealerAddress = dealer.Dealer?.Address ?? "N/A",
                    TotalOrders = successfulOrders.Count + pendingOrders.Count,
                    SuccessfulOrders = successfulOrders.Count,
                    PendingOrders = pendingOrders.Count,
                    TotalEarnings = totalEarnings,
                    TotalVehiclesSold = totalVehiclesSold
                };

                reports.Add(report);
            }

            return reports.OrderByDescending(r => r.TotalEarnings);
        }

        public async Task<SalesReportSummary> GetSalesReportSummaryAsync()
        {
            return await GetSalesReportSummaryAsync(null, null);
        }

        public async Task<SalesReportSummary> GetSalesReportSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            var dealers = allUsers.Where(u => u.Role == "DealerStaff" && u.Dealer != null);
            var allSuccessfulOrders = new List<Order>();
            var allPendingOrders = new List<Order>();

            foreach (var dealer in dealers)
            {
                var successfulOrders = await _orderRepository.GetSuccessfulOrderAsync(dealer.UserId, startDate, endDate);
                var pendingOrders = await _orderRepository.GetPendingOrderAsync(dealer.UserId);

                allSuccessfulOrders.AddRange(successfulOrders);
                allPendingOrders.AddRange(pendingOrders);
            }

            var totalEarnings = allSuccessfulOrders.Sum(order => order.TotalAmount);

            return new SalesReportSummary
            {
                TotalDealers = dealers.Count(),
                TotalOrders = allSuccessfulOrders.Count + allPendingOrders.Count,
                TotalEarnings = totalEarnings,
                ReportGeneratedDate = DateTime.Now
            };
        }
    }
}