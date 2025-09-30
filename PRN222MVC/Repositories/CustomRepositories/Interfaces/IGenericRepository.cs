using System.Linq.Expressions;

namespace Repositories.CustomRepositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> ExistsAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    }
}