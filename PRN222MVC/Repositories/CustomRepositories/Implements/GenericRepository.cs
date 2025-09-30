using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.CustomRepositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories.CustomRepositories.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly Prn222asm1Context _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(Prn222asm1Context context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;
                
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
    }
}