using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly Prn222asm1Context _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, object> _repositories = new();

        public RepositoryFactory(Prn222asm1Context context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
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

        public TRepository GetCustomRepository<TRepository>() where TRepository : class
        {
            if (!_repositories.ContainsKey(typeof(TRepository)))
            {
                var repo = _serviceProvider.GetRequiredService<TRepository>();
                _repositories[typeof(TRepository)] = repo;
            }
            return (TRepository)_repositories[typeof(TRepository)];
        }
    }
}
