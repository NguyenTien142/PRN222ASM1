using System;
using Repositories.Context;

namespace Repositories.WorkSeeds.Interfaces
{
    public interface IRepositoryFactory
    {
        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;

        TRepository GetCustomRepository<TRepository>() where TRepository : class;
    }
}
