using BusinessObject.BusinessObject.CustomerModels.Request;
using BusinessObject.BusinessObject.CustomerModels.Respond;

namespace Services.Intefaces;

public interface ICustomerService
{
    Task CreateCustomer(CreateCustomerRequestDto request);
    Task<List<GetCustomersResponseDto>?> GetAllCustomers();
    Task<GetCustomersResponseDto?> GetCustomerById(int customerId);
    Task UpdateCustomer(UpdateCustomerRequest request);
}