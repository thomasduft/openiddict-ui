using OpenIddict.EntityFrameworkCore.Models;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class ApplicationRepository<TContext>
    : EfRepository<OpenIddictEntityFrameworkCoreApplication, string>, IApplicationRepository
    where TContext : OpenIddictUIContext
  {
    public ApplicationRepository(TContext dbContext) : base(dbContext)
    {
    }
  }
}