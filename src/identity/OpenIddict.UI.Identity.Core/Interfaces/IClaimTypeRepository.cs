using System;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public interface IClaimTypeRepository : IAsyncRepository<ClaimType, Guid>
  { }
}