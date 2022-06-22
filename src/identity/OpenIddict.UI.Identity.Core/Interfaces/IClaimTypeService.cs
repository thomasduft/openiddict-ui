using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Core;

public interface IClaimTypeService
{
  Task<IEnumerable<ClaimTypeInfo>> GetClaimTypesAsync();

  Task<ClaimTypeInfo> GetAsync(Guid id);

  Task<Guid> CreateAsync(ClaimTypeParam model);

  Task UpdateAsync(ClaimTypeParam model);

  Task DeleteAsync(Guid id);
}
