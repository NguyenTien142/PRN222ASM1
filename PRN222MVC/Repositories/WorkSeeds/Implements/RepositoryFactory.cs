using Repositories.Context;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.WorkSeeds.Interfaces;

namespace Repositories.WorkSeeds.Implements
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly Prn222asm1Context _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public RepositoryFactory(Prn222asm1Context context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
        {
            if (!_repositories.ContainsKey(typeof(TEntity)))
            {
                var repo = new GenericRepository<TEntity>(_context);
                _repositories[typeof(TEntity)] = repo;
            }
            return (IGenericRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        public TRepository GetCustomRepository<TRepository>(Func<Prn222asm1Context, TRepository> factory)
            where TRepository : class
        {
            if (!_repositories.ContainsKey(typeof(TRepository)))
            {
                var repo = factory(_context);
                _repositories[typeof(TRepository)] = repo;
            }
            return (TRepository)_repositories[typeof(TRepository)];
        }
    }
}
