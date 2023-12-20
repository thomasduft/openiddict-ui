using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tomware.OpenIddict.UI.Infrastructure;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Api;

public interface IApplicationApiService
{
  Task<IEnumerable<ApplicationViewModel>> GetApplicationsAsync();

  Task<ApplicationViewModel> GetAsync(string clientId);

  Task<string> CreateAsync(ApplicationViewModel model);

  Task UpdateAsync(ApplicationViewModel model);

  Task DeleteAsync(string clientId);

  Task<ApplicationOptionsViewModel> GetOptionsAsync();
}

public class ApplicationApiService : IApplicationApiService
{
  private readonly IApplicationService _service;
  private readonly IOptions<OpenIddictUIApiOptions> _options;

  public ApplicationApiService(
    IApplicationService service,
    IOptions<OpenIddictUIApiOptions> options
  )
  {
    _service = service
      ?? throw new ArgumentNullException(nameof(service));
    _options = options
      ?? throw new ArgumentNullException(nameof(options));
  }

  public async Task<IEnumerable<ApplicationViewModel>> GetApplicationsAsync()
  {
    var items = await _service.GetApplicationsAsync();

    return items.Select(ToModel);
  }

  public async Task<ApplicationViewModel> GetAsync(string clientId)
  {
    ArgumentNullException.ThrowIfNull(clientId);

    var claimType = await _service.GetAsync(clientId);

    return claimType != null ? ToModel(claimType) : null;
  }

  public async Task<string> CreateAsync(ApplicationViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    var param = ToParam(model);

    return await _service.CreateAsync(param);
  }

  public async Task UpdateAsync(ApplicationViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    if (string.IsNullOrWhiteSpace(model.Id))
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var param = ToParam(model);

    await _service.UpdateAsync(param);
  }

  public async Task DeleteAsync(string clientId)
  {
    ArgumentNullException.ThrowIfNull(clientId);

    await _service.DeleteAsync(clientId);
  }

  public async Task<ApplicationOptionsViewModel> GetOptionsAsync()
  {
    var options = _options.Value;

    var model = new ApplicationOptionsViewModel
    {
      Permissions = options.Permissions
    };

    return await Task.FromResult(model);
  }

  private static ApplicationParam ToParam(ApplicationViewModel model)
  {
    return new ApplicationParam
    {
      Id = model.Id,
      ClientId = model.ClientId,
      DisplayName = model.DisplayName,
      ClientSecret = model.ClientSecret,
      Type = model.Type,
      RequirePkce = model.RequirePkce,
      RequireConsent = model.RequireConsent,
      Permissions = model.Permissions,
      RedirectUris = model.RedirectUris,
      PostLogoutRedirectUris = model.PostLogoutRedirectUris
    };
  }

  private static ApplicationViewModel ToModel(ApplicationInfo info)
  {
    return new ApplicationViewModel
    {
      Id = info.Id,
      ClientId = info.ClientId,
      DisplayName = info.DisplayName,
      Type = info.Type,
      RequirePkce = info.RequirePkce,
      RequireConsent = info.RequireConsent,
      Permissions = info.Permissions,
      RedirectUris = info.RedirectUris,
      PostLogoutRedirectUris = info.PostLogoutRedirectUris
    };
  }
}
