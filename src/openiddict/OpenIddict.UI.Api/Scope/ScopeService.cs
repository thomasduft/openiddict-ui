using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Infrastructure;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Api;

public interface IScopeApiService
{
  Task<IEnumerable<ScopeViewModel>> GetScopesAsync();

  Task<IEnumerable<string>> GetScopeNamesAsync();

  Task<ScopeViewModel> GetAsync(string id);

  Task<string> CreateAsync(ScopeViewModel model);

  Task UpdateAsync(ScopeViewModel model);

  Task DeleteAsync(string id);
}

public class ScopeApiService : IScopeApiService
{
  private readonly IScopeService _service;

  public ScopeApiService(IScopeService service)
  {
    _service = service;
  }

  public async Task<IEnumerable<ScopeViewModel>> GetScopesAsync()
  {
    var items = await _service.GetScopesAsync();

    return items.Select(c =>
    {
      return ToModel(c);
    });
  }

  public async Task<IEnumerable<string>> GetScopeNamesAsync()
  {
    var items = await _service.GetScopesAsync();

    return items.Select(i =>
    {
      return i.Name;
    });
  }

  public async Task<ScopeViewModel> GetAsync(string id)
  {
    if (id == null)
    {
      throw new ArgumentNullException(nameof(id));
    }

    var claimType = await _service.GetAsync(id);

    return claimType != null ? ToModel(claimType) : null;
  }

  public async Task<string> CreateAsync(ScopeViewModel model)
  {
    if (model == null)
    {
      throw new ArgumentNullException(nameof(model));
    }

    var param = ToParam(model);

    return await _service.CreateAsync(param);
  }

  public async Task UpdateAsync(ScopeViewModel model)
  {
    if (model == null)
    {
      throw new ArgumentNullException(nameof(model));
    }

    if (string.IsNullOrWhiteSpace(model.Id))
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var param = ToParam(model);

    await _service.UpdateAsync(param);
  }

  public async Task DeleteAsync(string id)
  {
    if (id == null)
    {
      throw new ArgumentNullException(nameof(id));
    }

    await _service.DeleteAsync(id);
  }

  private static ScopeParam ToParam(ScopeViewModel model) => SimpleMapper.From<ScopeViewModel, ScopeParam>(model);

  private static ScopeViewModel ToModel(ScopeInfo info) => SimpleMapper.From<ScopeInfo, ScopeViewModel>(info);
}
