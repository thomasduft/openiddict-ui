using System;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class ClaimTypeRepository<TContext>
    : EfRepository<ClaimType, Guid>, IClaimTypeRepository
    where TContext : OpenIddictUIContext
  {
    public ClaimTypeRepository(TContext dbContext) : base(dbContext)
    {
    }
  }
}