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
    public class UnitOfWork : IDisposable
    {
        private readonly Prn222asm1Context _context;
        private readonly RepositoryFactory _repositoryFactory;

        public UnitOfWork(Prn222asm1Context context)
        {
            _context = context;
            _repositoryFactory = new RepositoryFactory(_context);
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _repositoryFactory.GetGenericRepository<TEntity>();
        }

        public TRepository GetCustomRepository<TRepository>(Func<Prn222asm1Context, TRepository> factory)
            where TRepository : class
        {
            return _repositoryFactory.GetCustomRepository(factory);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
