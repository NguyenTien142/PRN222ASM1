using BusinessObject.BusinessObject.CustomerModels.Request;
using BusinessObject.BusinessObject.CustomerModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;

namespace Services.Implements;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateCustomer(CreateCustomerRequestDto request)
    {
        var customer = new Customer()
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
        };

        await _unitOfWork.GetRepository<Customer>().AddAsync(customer);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<GetCustomersResponseDto>?> GetAllCustomers()
    {
        var customers = await _customerRepository.GetAllCustomers();
        if (customers == null)
        {
            return null;
        }
        var response = customers.Select(c => new GetCustomersResponseDto()
        {
            CustomerId = c.CustomerId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
        }).ToList();

        return response;
    }

    public async Task<GetCustomersResponseDto?> GetCustomerById(int customerId)
    {
        var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(customerId);
        if (customer == null)
        {
            return null;
        }

        var response = new GetCustomersResponseDto()
        {
            CustomerId = customer.CustomerId,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
        };
        return response;
    }

    public async Task UpdateCustomer(UpdateCustomerRequest request)
    {
        var customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new Exception("Customer not found");
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;

        _unitOfWork.GetRepository<Customer>().Update(customer);
        await _unitOfWork.SaveAsync();
    }
}