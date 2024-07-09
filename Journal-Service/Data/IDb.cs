using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journal_Service;

public interface IDb
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;

    void Save();
    Task<int> SaveAsync();
}