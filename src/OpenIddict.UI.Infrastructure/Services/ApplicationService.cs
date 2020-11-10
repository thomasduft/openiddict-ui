using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Core;

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

      return items.Select(x => ToInfo(x));
    }

    public async Task<ApplicationInfo> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this._manager.FindByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<string> CreateAsync(ApplicationParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      // TODO: handle Json serialized properties
      var newEntity = new OpenIddictEntityFrameworkCoreApplication
      {
        ClientId = model.ClientId,
        DisplayName = model.DisplayName,
        ClientSecret = model.ClientSecret,
        ConsentType = model.ConsentType,
        Permissions = model.Permissions, // TODO: Permissions
        Properties = model.Properties, // TODO: Properties
        RedirectUris = model.RedirectUris, // TODO: RedirectUris
        PostLogoutRedirectUris = model.PostLogoutRedirectUris, // TODO: PostLogoutRedirectUris
        Requirements = model.Requirements, // TODO: Requirements
        Type = model.Type
      };

      await this._manager.CreateAsync(newEntity, newEntity.ClientSecret);

      return newEntity.Id;
    }

    public async Task UpdateAsync(ApplicationParam model)
    {
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var entity = await this._manager.FindByIdAsync(model.Id);

      // TODO: handle Json serialized properties
      SimpleMapper.Map<ApplicationParam, OpenIddictEntityFrameworkCoreApplication>(model, entity);

      await this._manager.UpdateAsync(entity, entity.ClientSecret);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this._manager.FindByIdAsync(id);

      await _manager.DeleteAsync(entity);
    }

    private ApplicationInfo ToInfo(OpenIddictEntityFrameworkCoreApplication entity)
    {
      return SimpleMapper.From<OpenIddictEntityFrameworkCoreApplication, ApplicationInfo>(entity);
    }
  }
}
