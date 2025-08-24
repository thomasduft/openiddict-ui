using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Infrastructure;

public interface IApplicationService
{
  public Task<IEnumerable<ApplicationInfo>> GetApplicationsAsync();

  public Task<ApplicationInfo> GetAsync(string id);

  public Task<string> CreateAsync(ApplicationParam model);

  public Task UpdateAsync(ApplicationParam model);

  public Task DeleteAsync(string id);
}
