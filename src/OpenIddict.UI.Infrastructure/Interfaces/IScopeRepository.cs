using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public interface IScopeRepository : IAsyncRepository<OpenIddictEntityFrameworkCoreScope, string>
  { }
}