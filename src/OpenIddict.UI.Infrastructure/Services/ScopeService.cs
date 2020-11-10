using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

      var entity = await this._manager.FindByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<string> CreateAsync(ScopeParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var newEntity = new OpenIddictEntityFrameworkCoreScope
      {
        Name = model.Name,
        DisplayName = model.DisplayName,
        Description = model.Description,
      };

      await this._manager.CreateAsync(newEntity);

      return newEntity.Id;
    }

    public async Task UpdateAsync(ScopeParam model)
    {
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var entity = await this._manager.FindByIdAsync(model.Id);

      SimpleMapper.Map<ScopeParam, OpenIddictEntityFrameworkCoreScope>(model, entity);

      await this._manager.UpdateAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this._manager.FindByIdAsync(id);

      await _manager.DeleteAsync(entity);
    }

    private ScopeInfo ToInfo(OpenIddictEntityFrameworkCoreScope entity)
    {
      return SimpleMapper.From<OpenIddictEntityFrameworkCoreScope, ScopeInfo>(entity);
    }
  }
}
