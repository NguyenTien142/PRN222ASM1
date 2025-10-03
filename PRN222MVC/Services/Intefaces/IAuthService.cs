
using BusinessObject.BusinessObject.UserModels.Request;
using BusinessObject.BusinessObject.UserModels.Respond;

namespace Services.Intefaces
{
    public interface IAuthService
    {
        Task<LoginRespond> LoginAsync(LoginRequest request);
        Task<RegisterRespond> RegisterAsync(RegisterRequest request);
        string HashPassword(string password);
    }
}