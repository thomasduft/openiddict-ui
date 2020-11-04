using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class EfRepository<TEntity, TKey> : IAsyncRepository<TEntity, TKey>
    where TEntity : class
  {
    protected readonly OpenIddictUIContext DbContext;

    public EfRepository(OpenIddictUIContext dbContext)
    {
      this.DbContext = dbContext;
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey id)
    {
      return await this.DbContext.Set<TEntity>()
        .FindAsync(id);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
    {
      return await this.ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
      return await this.ApplySpecification(spec).CountAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      this.DbContext.Set<TEntity>().Add(entity);

      await this.DbContext.SaveChangesAsync();

      return entity;
    }

    public async System.Threading.Tasks.Task UpdateAsync(TEntity entity)
    {
      this.DbContext.Entry(entity).State = EntityState.Modified;

      await this.DbContext.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(TEntity entity)
    {
      this.DbContext.Set<TEntity>().Remove(entity);

      await this.DbContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
      return SpecificationEvaluator<TEntity>
        .GetQuery(this.DbContext.Set<TEntity>().AsQueryable(), spec);
    }
  }
}
