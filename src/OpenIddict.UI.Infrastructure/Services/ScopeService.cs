using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class ScopeService : IScopeService
  {
    private readonly IScopeRepository _repository;
    private readonly OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> _manager;

    public ScopeService(
      IScopeRepository repository,
      OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> manager
    )
    {
      _repository = repository
        ?? throw new ArgumentNullException(nameof(repository));
      _manager = manager
        ?? throw new ArgumentNullException(nameof(manager));
    }

    public async Task<IEnumerable<ScopeInfo>> GetScopesAsync()
    {
      var items = await _repository.ListAsync(new AllScopes());

      return items.Select(x => ToInfo(x));
    }

    public async Task<ScopeInfo> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await _manager.FindByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<string> CreateAsync(ScopeParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var entity = await _manager.FindByNameAsync(model.Name);
      if (entity == null)
      {
        // create new entity
        var newEntity = new OpenIddictEntityFrameworkCoreScope
        {
          Name = model.Name,
          DisplayName = model.DisplayName,
          Description = model.Description
        };

        HandleCustomProperties(model, newEntity);

        await _manager.CreateAsync(newEntity);

        return newEntity.Id;
      }

      // update existing entity
      model.Id = entity.Id;
      await UpdateAsync(model);

      return entity.Id;
    }

    public async Task UpdateAsync(ScopeParam model)
    {
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var entity = await _manager.FindByIdAsync(model.Id);

      SimpleMapper.Map<ScopeParam, OpenIddictEntityFrameworkCoreScope>(model, entity);

      HandleCustomProperties(model, entity);

      await _manager.UpdateAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await _manager.FindByIdAsync(id);

      await _manager.DeleteAsync(entity);
    }

    private ScopeInfo ToInfo(OpenIddictEntityFrameworkCoreScope entity)
    {
      var info = SimpleMapper
        .From<OpenIddictEntityFrameworkCoreScope, ScopeInfo>(entity);

      info.Resources = entity.Resources != null
        ? JsonSerializer.Deserialize<List<string>>(entity.Resources)
        : new List<string>();

      return info;
    }

    private void HandleCustomProperties(
      ScopeParam model,
      OpenIddictEntityFrameworkCoreScope entity
    )
    {
      entity.Resources = JsonSerializer.Serialize(model.Resources);
    }
  }
}
