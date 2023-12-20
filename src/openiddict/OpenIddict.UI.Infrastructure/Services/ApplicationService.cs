using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Suite.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Infrastructure;

public class ApplicationService : IApplicationService
{
  private readonly IApplicationRepository _repository;
  private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _manager;

  public ApplicationService(
    IApplicationRepository repository,
    OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager
  )
  {
    _repository = repository
      ?? throw new ArgumentNullException(nameof(repository));
    _manager = manager
      ?? throw new ArgumentNullException(nameof(manager));
  }

  public async Task<IEnumerable<ApplicationInfo>> GetApplicationsAsync()
  {
    var items = await _repository.ListAsync(new AllApplications());

    return items.Select(ToInfo);
  }

  public async Task<ApplicationInfo> GetAsync(string id)
  {
    ArgumentNullException.ThrowIfNull(id);

    var entity = await _manager.FindByIdAsync(id);

    return entity != null ? ToInfo(entity) : null;
  }

  public async Task<string> CreateAsync(ApplicationParam model)
  {
    ArgumentNullException.ThrowIfNull(model);

    var entity = await _manager.FindByClientIdAsync(model.ClientId);
    if (entity == null)
    {
      // create new entity
      var newEntity = new OpenIddictEntityFrameworkCoreApplication();
      MapProperties(model, newEntity);

      await _manager.CreateAsync(newEntity, model.ClientSecret);

      return newEntity.Id;
    }

    // update existing entity
    model.Id = entity.Id;
    await UpdateAsync(model);

    return entity.Id;
  }

  public async Task UpdateAsync(ApplicationParam model)
  {
    if (string.IsNullOrWhiteSpace(model.Id))
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var entity = await _manager.FindByIdAsync(model.Id);
    MapProperties(model, entity);

    await _manager.UpdateAsync(entity, model.ClientSecret);
  }

  public async Task DeleteAsync(string id)
  {
    ArgumentNullException.ThrowIfNull(id);

    var entity = await _manager.FindByIdAsync(id);

    await _manager.DeleteAsync(entity);
  }

  private static ApplicationInfo ToInfo(OpenIddictEntityFrameworkCoreApplication entity)
  {
    return new ApplicationInfo
    {
      Id = entity.Id,
      ClientId = entity.ClientId,
      DisplayName = entity.DisplayName,
      Type = entity.ClientType,
      RequirePkce = entity.Requirements != null && JsonSerializer
      .Deserialize<List<string>>(entity.Requirements)
      .Contains(Requirements.Features.ProofKeyForCodeExchange),
      RequireConsent = entity.ConsentType == ConsentTypes.Explicit,
      Permissions = entity.Permissions != null
      ? JsonSerializer.Deserialize<List<string>>(entity.Permissions)
      : [],
      RedirectUris = entity.RedirectUris != null
      ? JsonSerializer.Deserialize<List<string>>(entity.RedirectUris)
      : [],
      PostLogoutRedirectUris = entity.PostLogoutRedirectUris != null
      ? JsonSerializer.Deserialize<List<string>>(entity.PostLogoutRedirectUris)
      : []
    };
  }

  private static void MapProperties(
    ApplicationParam model,
    OpenIddictEntityFrameworkCoreApplication entity
  )
  {
    entity.ClientId = model.ClientId;
    entity.DisplayName = model.DisplayName;
    entity.ClientType = model.Type;
    entity.ConsentType = model.RequireConsent ? ConsentTypes.Explicit : ConsentTypes.Implicit;
    entity.Permissions = JsonSerializer.Serialize(model.Permissions);
    entity.RedirectUris = JsonSerializer.Serialize(model.RedirectUris);
    entity.PostLogoutRedirectUris = JsonSerializer.Serialize(model.PostLogoutRedirectUris);
    entity.Requirements = model.RequirePkce
      ? JsonSerializer.Serialize(new List<string> { Requirements.Features.ProofKeyForCodeExchange })
      : null;
  }
}
