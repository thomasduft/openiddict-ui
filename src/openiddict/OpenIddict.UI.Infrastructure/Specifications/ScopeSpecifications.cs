using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public sealed class AllScopes : BaseSpecification<OpenIddictEntityFrameworkCoreScope>
  {
    public AllScopes()
    {
      ApplyOrderBy(x => x.Name);
      ApplyNoTracking();
    }
  }
}
