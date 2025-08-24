using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Infrastructure;

public interface IScopeService
{
  public Task<IEnumerable<ScopeInfo>> GetScopesAsync();

  public Task<ScopeInfo> GetAsync(string id);

  public Task<string> CreateAsync(ScopeParam model);

  public Task UpdateAsync(ScopeParam model);

  public Task DeleteAsync(string id);
}
