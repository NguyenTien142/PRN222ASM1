using BusinessObject.BusinessObject.OrderModels.Request;
using BusinessObject.BusinessObject.OrderModels.Respond;
using BusinessObject.BusinessObject.OrderModels.Response;

namespace Services.Intefaces;

public interface IOrderServices
{
    Task CreateOrder(int userId, CreateOrderRequestDto request);
    Task<List<GetOrdersResponseDto>> GetAllOrders(int userId);
    Task<GetOrderByIdResponseDto?> GetOrderById(int userId, int orderId);
    Task<List<GetSuccessfulOrderResponse>> GetSuccessfulOrderAsync(int userId);
    Task<List<GetPendingOrderResponse>> GetPendingOrderAsync(int userId);
    Task<List<GetPendingOrderResponse>> GetAllPendingOrdersAsync();
    Task<GetOrderByIdResponseDto?> GetOrderByIdForApproval(int orderId);
    Task<bool> ApproveOrderAsync(int orderId);
    Task<bool> RejectOrderAsync(int orderId);
    Task<decimal> GetTotalEarningsByUserAsync(int userId);
}