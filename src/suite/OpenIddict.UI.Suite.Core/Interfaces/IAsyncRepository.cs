using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Suite.Core
{
  public interface IAsyncRepository<TEntity, TKey> 
    where TEntity : class
  {
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<int> CountAsync(ISpecification<TEntity> spec);
  }
}
