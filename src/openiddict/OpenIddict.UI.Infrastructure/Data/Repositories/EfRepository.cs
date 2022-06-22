using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class EfRepository<TEntity, TKey> : IAsyncRepository<TEntity, TKey>
    where TEntity : class
  {
    protected readonly OpenIddictUIContext Context;

    public EfRepository(OpenIddictUIContext context)
    {
      Context = context;
    }

    public async Task<TEntity> GetByIdAsync(TKey id)
    {
      return await Context.Set<TEntity>()
        .FindAsync(id);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
    {
      return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
      return await ApplySpecification(spec).CountAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await Context.Set<TEntity>().AddAsync(entity);

      await Context.SaveChangesAsync();

      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      Context.Entry(entity).State = EntityState.Modified;

      await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
      Context.Set<TEntity>().Remove(entity);

      await Context.SaveChangesAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
      return SpecificationEvaluator<TEntity>
        .GetQuery(Context.Set<TEntity>().AsQueryable(), spec);
    }
  }
}