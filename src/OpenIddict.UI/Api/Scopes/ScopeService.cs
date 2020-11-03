using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI
{
  public interface IScopeService
  {
    Task<IEnumerable<ScopeViewModel>> GetScopesAsync();

    Task<IEnumerable<string>> GetScopeNamesAsync();

    Task<ScopeViewModel> GetAsync(string name);

    Task<string> CreateAsync(ScopeViewModel model);

    Task UpdateAsync(ScopeViewModel model);

    Task DeleteAsync(string name);
  }

  // public class ScopeService : IScopeService
  // {
  //   private readonly ConfigurationDbContext context;

  //   public ScopeService(ConfigurationDbContext context)
  //   {
  //     this.context = context;
  //   }

  //   public async Task<IEnumerable<ScopeViewModel>> GetScopesAsync()
  //   {
  //     var items = await this.LoadAll()
  //      .AsNoTracking()
  //      .ToListAsync();

  //     return items.Select(x => ToModel(x));
  //   }

  //   public async Task<IEnumerable<string>> GetScopeNamesAsync()
  //   {
  //     var apiScopes = await this.context.ApiScopes
  //       .OrderBy(x => x.Name)
  //       .AsNoTracking()
  //       .ToListAsync();

  //     var identityResources = await this.context.IdentityResources
  //       .OrderBy(x => x.Name)
  //       .AsNoTracking()
  //       .ToListAsync();

  //     return apiScopes.Select(x => x.Name)
  //       .Union(identityResources.Select(x => x.Name))
  //       .Distinct();
  //   }

  //   public async Task<ScopeViewModel> GetAsync(string name)
  //   {
  //     if (name == null) throw new ArgumentNullException(nameof(name));

  //     var apiScope = await this.GetApiScopeByName(name);

  //     return apiScope != null ? ToModel(apiScope) : null;
  //   }

  //   public async Task<string> CreateAsync(ScopeViewModel model)
  //   {
  //     if (model is null) throw new System.ArgumentNullException(nameof(model));

  //     var apiScope = new ApiScope
  //     {
  //       Enabled = model.Enabled,
  //       Name = model.Name,
  //       DisplayName = model.DisplayName,
  //       Description = model.Description,
  //       Required = model.Required,
  //       ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
  //       Emphasize = model.Emphasize,
  //     };

  //     HandleCollectionProperties(model, apiScope);

  //     this.context.ApiScopes.Add(apiScope);

  //     await this.context.SaveChangesAsync();

  //     return apiScope.Name;
  //   }

  //   public async Task UpdateAsync(ScopeViewModel model)
  //   {
  //     if (model == null) throw new ArgumentNullException(nameof(model));

  //     var apiScope = await this.GetApiScopeId(model.Id.Value);
  //     if (apiScope == null) throw new ArgumentNullException(nameof(apiScope));

  //     apiScope.Enabled = model.Enabled;
  //     apiScope.Name = model.Name;
  //     apiScope.DisplayName = model.DisplayName;
  //     apiScope.Description = model.Description;
  //     apiScope.Required = model.Required;
  //     apiScope.ShowInDiscoveryDocument = model.ShowInDiscoveryDocument;
  //     apiScope.Emphasize = model.Emphasize;

  //     HandleCollectionProperties(model, apiScope);

  //     this.context.ApiScopes.Update(apiScope);

  //     await this.context.SaveChangesAsync();
  //   }

  //   public async Task DeleteAsync(string name)
  //   {
  //     if (name == null) throw new ArgumentNullException(nameof(name));

  //     var apiScope = await this.context.ApiScopes
  //       .FirstOrDefaultAsync(c => c.Name == name);

  //     var clients = await this.context.Clients
  //             .Include(x => x.AllowedScopes)
  //             .ToListAsync();
  //     foreach (var client in clients)
  //     {
  //       client.AllowedScopes.RemoveAll(x => apiScope.Name == x.Scope);
  //     }

  //     var apiResources = await this.context.ApiResources
  //             .Include(x => x.Scopes)
  //             .ToListAsync();
  //     foreach (var apiResource in apiResources)
  //     {
  //       apiResource.Scopes.RemoveAll(x => apiScope.Name == x.Scope);
  //     }

  //     this.context.ApiScopes.Remove(apiScope);

  //     await this.context.SaveChangesAsync();
  //   }

  //   private IOrderedQueryable<ApiScope> LoadAll()
  //   {
  //     return this.context.ApiScopes
  //             .Include(x => x.UserClaims)
  //             .Include(x => x.Properties)
  //             .OrderBy(x => x.Name);
  //   }

  //   private async Task<ApiScope> GetApiScopeByName(string name)
  //   {
  //     List<ApiScope> items = await this.LoadAll()
  //      .Where(x => x.Name == name)
  //      .ToListAsync();

  //     return items.Count() == 1 ? items.First() : null;
  //   }

  //   private async Task<ApiScope> GetApiScopeId(int id)
  //   {
  //     List<ApiScope> items = await this.LoadAll()
  //      .Where(x => x.Id == id)
  //      .ToListAsync();

  //     return items.Count() == 1 ? items.First() : null;
  //   }

  //   private ScopeViewModel ToModel(ApiScope entity)
  //   {
  //     return new ScopeViewModel
  //     {
  //       Id = entity.Id,
  //       Enabled = entity.Enabled,
  //       Name = entity.Name,
  //       DisplayName = entity.DisplayName,
  //       Description = entity.Description,
  //       Required = entity.Required,
  //       ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
  //       Emphasize = entity.Emphasize,
  //       UserClaims = entity.UserClaims
  //         .Select(x => x.Type).ToList()
  //     };
  //   }

  //   private void HandleCollectionProperties(ScopeViewModel model, ApiScope apiScope)
  //   {
  //     // deassign them
  //     if (apiScope.UserClaims != null) apiScope.UserClaims.Clear();

  //     // assign them
  //     apiScope.UserClaims = model.UserClaims
  //       .Select(x => new ApiScopeClaim
  //       {
  //         Scope = apiScope,
  //         Type = x
  //       }).ToList();
  //   }
  // }
}