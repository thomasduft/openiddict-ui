using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Core
{
  public interface IClaimTypeService
  {
    Task<IEnumerable<ClaimTypeInfo>> GetClaimTypesAsync();

    Task<ClaimTypeInfo> GetAsync(Guid id);

    Task<bool> ClaimTypeExists(string name);

    Task<Guid> CreateAsync(ClaimTypeParam model);

    Task UpdateAsync(ClaimTypeParam model);

    Task DeleteAsync(Guid id);
  }
}