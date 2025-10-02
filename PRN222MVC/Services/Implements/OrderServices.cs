using BusinessObject.BusinessObject.OrderModels.Request;
using BusinessObject.BusinessObject.OrderModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;

namespace Services.Implements;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    public OrderService(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task CreateOrder(int userId, CreateOrderRequestDto request)
    {
        var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new Exception("Customer not found");
        }

        var order = new Order()
        {
            UserId = user.UserId,
            CustomerId = request.CustomerId,
            OrderDate = DateTime.Now,
            Status = "Pending",
        };
        foreach (var orderVehicle in request.OrderVehicles)
        {
            var vehicle = await _unitOfWork.GetRepository<Vehicle>().GetByIdAsync(orderVehicle.VehicleId);
            if (vehicle == null)
            {
                throw new Exception($"Vehicle with ID {orderVehicle.VehicleId} not found");
            }

            order.OrderVehicles.Add(new OrderVehicle()
            {
                OrderId = order.OrderId,
                Quantity = orderVehicle.Quantity,
                VehicleId = vehicle.VehicleId,
                UnitPrice = vehicle.Price * orderVehicle.Quantity,
            });
            order.TotalAmount += vehicle.Price * orderVehicle.Quantity;
        }

        await _unitOfWork.GetRepository<Order>().AddAsync(order);
        await _unitOfWork.SaveAsync();


    }

    public async Task<List<GetOrdersResponseDto>> GetAllOrders(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserId(userId);

        var response = orders != null ? orders.Select(o => new GetOrdersResponseDto()
        {
            OrderId = o.OrderId,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            CustomerName = o.Customer.Name
        }).ToList() : new List<GetOrdersResponseDto>();

        return response;
    }

    public async Task<GetOrderByIdResponseDto?> GetOrderById(int userId, int orderId)
    {
        var order = await _orderRepository.GetOrderById(userId, orderId);

        if (order == null)
        {
            return null;
        }

        var response = new GetOrderByIdResponseDto()
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Name,
            OrderVehicles = order.OrderVehicles.Select(x => new GetOrderVehicleResponseDto()
            {
                VehicleId = x.VehicleId,
                VehicleName = x.Vehicle.Model,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        return response;
    }
}