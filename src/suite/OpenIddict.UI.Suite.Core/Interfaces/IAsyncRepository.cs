using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Suite.Core;

public interface IAsyncRepository<TEntity, TKey>
  where TEntity : class
{
  public Task<TEntity> GetByIdAsync(TKey id);
  public Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);
  public Task<TEntity> AddAsync(TEntity entity);
  public Task UpdateAsync(TEntity entity);
  public Task DeleteAsync(TEntity entity);
  public Task<int> CountAsync(ISpecification<TEntity> spec);
}
