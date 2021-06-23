using System;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public interface IClaimTypeRepository : IAsyncRepository<ClaimType, Guid>
  { }
}