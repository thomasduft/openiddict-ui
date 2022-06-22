using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public interface IScopeRepository
    : IAsyncRepository<OpenIddictEntityFrameworkCoreScope, string>
  { }
}