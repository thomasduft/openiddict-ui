using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Infrastructure;

public class EfRepository<TEntity, TKey> : IAsyncRepository<TEntity, TKey>
  where TEntity : class
{
  private readonly OpenIddictUIIdentityContext _context;

  public EfRepository(OpenIddictUIIdentityContext context)
  {
    _context = context;
  }

  public async Task<TEntity> GetByIdAsync(TKey id)
  {
    return await _context.Set<TEntity>()
      .FindAsync(id);
  }

  public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec) => await ApplySpecification(spec).ToListAsync();

  public async Task<int> CountAsync(ISpecification<TEntity> spec) => await ApplySpecification(spec).CountAsync();

  public async Task<TEntity> AddAsync(TEntity entity)
  {
    await _context.Set<TEntity>().AddAsync(entity);

    await _context.SaveChangesAsync();

    return entity;
  }

  public async Task UpdateAsync(TEntity entity)
  {
    _context.Entry(entity).State = EntityState.Modified;

    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(TEntity entity)
  {
    _context.Set<TEntity>().Remove(entity);

    await _context.SaveChangesAsync();
  }

  private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
  {
    return SpecificationEvaluator<TEntity>
      .GetQuery(_context.Set<TEntity>().AsQueryable(), spec);
  }
}
