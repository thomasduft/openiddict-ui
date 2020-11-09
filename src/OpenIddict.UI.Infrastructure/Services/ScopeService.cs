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
    private readonly OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> _scopeManager;

    public ScopeService(
      IScopeRepository repository,
      OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope> scopeManager
    )
    {
      _repository = repository
        ?? throw new ArgumentNullException(nameof(repository));
      _scopeManager = scopeManager
        ?? throw new ArgumentNullException(nameof(scopeManager));
    }

    public async Task<IEnumerable<ScopeInfo>> GetScopesAsync()
    {
      var items = await _repository.ListAsync(new AllScopes());

      return items.Select(x => ToInfo(x));
    }

    public async Task<ScopeInfo> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this._scopeManager.FindByIdAsync(id);

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

      await this._scopeManager.CreateAsync(newEntity);

      return newEntity.Id;
    }

    public async Task UpdateAsync(ScopeParam model)
    {
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var entity = await this._scopeManager.FindByIdAsync(model.Id);

      SimpleMapper.Map<ScopeParam, OpenIddictEntityFrameworkCoreScope>(model, entity);

      await this._scopeManager.UpdateAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this._scopeManager.FindByIdAsync(id);

      await _scopeManager.DeleteAsync(entity);
    }

    private ScopeInfo ToInfo(OpenIddictEntityFrameworkCoreScope entity)
    {
      return SimpleMapper.From<OpenIddictEntityFrameworkCoreScope, ScopeInfo>(entity);
    }
  }
}
