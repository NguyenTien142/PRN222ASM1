using System;
using Repositories.Context;

namespace Repositories.WorkSeeds.Interfaces
{
    public interface IRepositoryFactory
    {
        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
        TRepository GetCustomRepository<TRepository>(Func<Prn222asm1Context, TRepository> factory)
            where TRepository : class;
    }
}
