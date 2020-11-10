using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Core;
using tomware.OpenIddict.UI.Infrastructure;

namespace tomware.OpenIddict.UI.Api
{
  public interface IApplicationApiService
  {
    Task<IEnumerable<ApplicationViewModel>> GetClientsAsync();

    Task<ApplicationViewModel> GetAsync(string clientId);

    Task<string> CreateAsync(ApplicationViewModel model);

    Task UpdateAsync(ApplicationViewModel model);

    Task DeleteAsync(string clientId);
  }

  public class ApplicationApiService : IApplicationApiService
  {
    private readonly IApplicationService _service;

    public ApplicationApiService(
      IApplicationService service
    )
    {
      _service = service
        ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<IEnumerable<ApplicationViewModel>> GetClientsAsync()
    {
      var items = await _service.GetApplicationsAsync();

      return items.Select(c => ToModel(c));
    }

    public async Task<ApplicationViewModel> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var claimType = await _service.GetAsync(id);

      return claimType != null ? ToModel(claimType) : null;
    }

    public async Task<string> CreateAsync(ApplicationViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var param = ToParam(model);

      return await _service.CreateAsync(param);
    }

    public async Task UpdateAsync(ApplicationViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var param = ToParam(model);

      await _service.UpdateAsync(param);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      await _service.DeleteAsync(id);
    }

    private ApplicationParam ToParam(ApplicationViewModel model)
    {
      return SimpleMapper.From<ApplicationViewModel, ApplicationParam>(model);
    }

    private ApplicationViewModel ToModel(ApplicationInfo info)
    {
      return SimpleMapper.From<ApplicationInfo, ApplicationViewModel>(info);
    }
  }
}