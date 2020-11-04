using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Core
{
  public interface IClaimTypeService
  {
    Task<IEnumerable<ClaimeTypeInfo>> GetClaimTypesAsync();

    Task<ClaimeTypeInfo> GetAsync(Guid id);

    Task<Guid> CreateAsync(ClaimeTypeParam model);

    Task UpdateAsync(ClaimeTypeParam model);

    Task DeleteAsync(Guid id);
  }
}