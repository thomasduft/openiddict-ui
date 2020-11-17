using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Infrastructure
{
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

      return items.Select(x => ToListInfo(x));
    }

    public async Task<ApplicationInfo> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await _manager.FindByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<string> CreateAsync(ApplicationParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var entity = await _manager.FindByClientIdAsync(model.ClientId);
      if (entity == null)
      {
        // create new entity
        var newEntity = new OpenIddictEntityFrameworkCoreApplication
        {
          ClientId = model.ClientId,
          DisplayName = model.DisplayName,
          ClientSecret = model.ClientSecret,
          Type = model.Type
        };

        HandleCustomProperties(model, newEntity);

        await _manager.CreateAsync(newEntity, newEntity.ClientSecret);

        return newEntity.Id;
      }

      // update existing entity
      model.Id = entity.Id;
      SimpleMapper.Map<ApplicationParam, OpenIddictEntityFrameworkCoreApplication>(model, entity);
      await _manager.UpdateAsync(entity);

      return entity.Id;
    }

    public async Task UpdateAsync(ApplicationParam model)
    {
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var entity = await _manager.FindByIdAsync(model.Id);

      SimpleMapper.Map<ApplicationParam, OpenIddictEntityFrameworkCoreApplication>(model, entity);

      HandleCustomProperties(model, entity);

      await _manager.UpdateAsync(entity, entity.ClientSecret);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await _manager.FindByIdAsync(id);

      await _manager.DeleteAsync(entity);
    }

    private ApplicationInfo ToListInfo(OpenIddictEntityFrameworkCoreApplication entity)
    {
      return SimpleMapper.From<OpenIddictEntityFrameworkCoreApplication, ApplicationInfo>(entity);
    }

    private ApplicationInfo ToInfo(OpenIddictEntityFrameworkCoreApplication entity)
    {
      var info = SimpleMapper.From<OpenIddictEntityFrameworkCoreApplication, ApplicationInfo>(entity);

      info.RequireConsent = entity.ConsentType == ConsentTypes.Explicit;
      info.Permissions = entity.Permissions != null
        ? JsonSerializer.Deserialize<List<string>>(entity.Permissions)
        : new List<string>();
      info.RedirectUris = entity.RedirectUris != null
        ? JsonSerializer.Deserialize<List<string>>(entity.RedirectUris)
        : new List<string>();
      info.PostLogoutRedirectUris = entity.PostLogoutRedirectUris != null
        ? JsonSerializer.Deserialize<List<string>>(entity.PostLogoutRedirectUris)
        : new List<string>();
      info.RequirePkce = entity.Requirements != null
        ? JsonSerializer.Deserialize<List<string>>(entity.Requirements)
        .Contains(Requirements.Features.ProofKeyForCodeExchange)
        : false;

      return info;
    }

    private static void HandleCustomProperties(
      ApplicationParam model,
      OpenIddictEntityFrameworkCoreApplication entity
    )
    {
      entity.ConsentType = model.RequireConsent ? ConsentTypes.Explicit : ConsentTypes.Implicit;
      entity.Permissions = JsonSerializer.Serialize(model.Permissions);
      entity.RedirectUris = JsonSerializer.Serialize(model.RedirectUris);
      entity.PostLogoutRedirectUris = JsonSerializer.Serialize(model.PostLogoutRedirectUris);
      entity.Requirements = model.RequirePkce ? JsonSerializer.Serialize(new List<string> {
        Requirements.Features.ProofKeyForCodeExchange
      }) : null;
    }
  }
}
