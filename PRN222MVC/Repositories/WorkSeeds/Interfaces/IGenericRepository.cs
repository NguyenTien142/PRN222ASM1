using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.WorkSeeds.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(object id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
        void DeleteAsync(TEntity entity);
        Task<bool> ExistsAsync(object id);
    }
}
