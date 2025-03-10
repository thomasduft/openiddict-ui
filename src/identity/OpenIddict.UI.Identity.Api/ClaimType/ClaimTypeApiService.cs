using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Identity.Core;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Api;

public interface IClaimTypeApiService
{
  public Task<IEnumerable<ClaimTypeViewModel>> GetClaimTypesAsync();

  public Task<ClaimTypeViewModel> GetAsync(Guid id);

  public Task<Guid> CreateAsync(ClaimTypeViewModel model);

  public Task UpdateAsync(ClaimTypeViewModel model);

  public Task DeleteAsync(Guid id);
}

public class ClaimTypeApiService : IClaimTypeApiService
{
  private readonly IClaimTypeService _service;

  public ClaimTypeApiService(IClaimTypeService service)
  {
    _service = service
      ?? throw new ArgumentNullException(nameof(service));
  }

  public async Task<IEnumerable<ClaimTypeViewModel>> GetClaimTypesAsync()
  {
    var items = await _service.GetClaimTypesAsync();

    return items.Select(ToModel);
  }

  public async Task<ClaimTypeViewModel> GetAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentNullException(nameof(id));
    }

    var claimType = await _service.GetAsync(id);

    return claimType != null ? ToModel(claimType) : null;
  }

  public async Task<Guid> CreateAsync(ClaimTypeViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    var param = ToParam(model);

    return await _service.CreateAsync(param);
  }

  public async Task UpdateAsync(ClaimTypeViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    if (!model.Id.HasValue)
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var param = ToParam(model);

    await _service.UpdateAsync(param);
  }

  public async Task DeleteAsync(Guid id)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentNullException(nameof(id));
    }

    await _service.DeleteAsync(id);
  }

  private static ClaimTypeParam ToParam(ClaimTypeViewModel model) => SimpleMapper.From<ClaimTypeViewModel, ClaimTypeParam>(model);

  private static ClaimTypeViewModel ToModel(ClaimTypeInfo info)
  {
    var vm = SimpleMapper.From<ClaimTypeInfo, ClaimTypeViewModel>(info);
    vm.Id = info.Id;
    return vm;
  }
}
