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
    protected readonly OpenIddictUIContext Context;

    public EfRepository(OpenIddictUIContext context)
    {
      this.Context = context;
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey id)
    {
      return await this.Context.Set<TEntity>()
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
      this.Context.Set<TEntity>().Add(entity);

      await this.Context.SaveChangesAsync();

      return entity;
    }

    public async System.Threading.Tasks.Task UpdateAsync(TEntity entity)
    {
      this.Context.Entry(entity).State = EntityState.Modified;

      await this.Context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(TEntity entity)
    {
      this.Context.Set<TEntity>().Remove(entity);

      await this.Context.SaveChangesAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
      return SpecificationEvaluator<TEntity>
        .GetQuery(this.Context.Set<TEntity>().AsQueryable(), spec);
    }
  }
}
