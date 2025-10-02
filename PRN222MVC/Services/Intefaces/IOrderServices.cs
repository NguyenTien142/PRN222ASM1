using BusinessObject.BusinessObject.OrderModels.Request;
using BusinessObject.BusinessObject.OrderModels.Respond;

namespace Services.Intefaces;

public interface IOrderService
{
    Task CreateOrder(int userId, CreateOrderRequestDto request);
    Task<List<GetOrdersResponseDto>> GetAllOrders(int userId);

    Task<GetOrderByIdResponseDto?> GetOrderById(int userId, int orderId);
}