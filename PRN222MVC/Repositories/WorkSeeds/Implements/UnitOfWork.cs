using Repositories.Context;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.WorkSeeds.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Prn222asm1Context _context;
        private readonly IRepositoryFactory _repositoryFactory;

        public UnitOfWork(Prn222asm1Context context, IServiceProvider serviceProvider)
        {
            _context = context;
            _repositoryFactory = new RepositoryFactory(_context, serviceProvider);
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _repositoryFactory.GetGenericRepository<TEntity>();
        }

        public TRepository GetCustomRepository<TRepository>() where TRepository : class
        {
            return _repositoryFactory.GetCustomRepository<TRepository>();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
