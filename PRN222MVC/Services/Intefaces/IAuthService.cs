using Repositories.Model;

namespace Services.Intefaces
{
    public interface IAuthService
    {
        Task<(User? user, string? token)> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        string HashPassword(string password);
    }
}