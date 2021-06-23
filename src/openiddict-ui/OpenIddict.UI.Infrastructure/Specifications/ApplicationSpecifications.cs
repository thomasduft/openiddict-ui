using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public sealed class AllApplications : BaseSpecification<OpenIddictEntityFrameworkCoreApplication>
  {
    public AllApplications()
    {
      ApplyOrderBy(x => x.DisplayName);
      ApplyNoTracking();
    }
  }
}
