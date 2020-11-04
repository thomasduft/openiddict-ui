using System;

namespace tomware.OpenIddict.UI.Core
{
  public interface IClaimTypeRepository : IAsyncRepository<ClaimType, Guid>
  { }
}