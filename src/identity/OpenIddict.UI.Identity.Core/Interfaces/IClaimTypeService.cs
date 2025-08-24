using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Core;

public interface IClaimTypeService
{
  public Task<IEnumerable<ClaimTypeInfo>> GetClaimTypesAsync();

  public Task<ClaimTypeInfo> GetAsync(Guid id);

  public Task<Guid> CreateAsync(ClaimTypeParam model);

  public Task UpdateAsync(ClaimTypeParam model);

  public Task DeleteAsync(Guid id);
}
