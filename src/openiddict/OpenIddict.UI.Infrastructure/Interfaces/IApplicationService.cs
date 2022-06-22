using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public interface IApplicationService
  {
    Task<IEnumerable<ApplicationInfo>> GetApplicationsAsync();

    Task<ApplicationInfo> GetAsync(string id);

    Task<string> CreateAsync(ApplicationParam model);

    Task UpdateAsync(ApplicationParam model);

    Task DeleteAsync(string id);
  }
}