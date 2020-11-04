using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI.Api
{
  public interface IClientService
  {
    Task<IEnumerable<ClientViewModel>> GetClientsAsync();

    Task<ClientViewModel> GetAsync(string clientId);

    Task<string> CreateAsync(ClientViewModel model);

    Task UpdateAsync(ClientViewModel model);

    Task DeleteAsync(string clientId);
  }

  // public class ClientService : IClientService
  // {
  //   private readonly ConfigurationDbContext context;

  //   public ClientService(
  //     ConfigurationDbContext context
  //   )
  //   {
  //     this.context = context;
  //   }

  //   public async Task<IEnumerable<ClientViewModel>> GetClientsAsync()
  //   {
  //     var items = await LoadAll()
  //       .AsNoTracking()
  //       .ToListAsync();

  //     return items.Select(x => ToModel(x));
  //   }

  //   public async Task<ClientViewModel> GetAsync(string clientId)
  //   {
  //     if (clientId == null) throw new ArgumentNullException(nameof(clientId));

  //     var client = await GetClientByClientId(clientId);

  //     return client != null ? ToModel(client) : null;
  //   }

  //   public async Task<string> CreateAsync(ClientViewModel model)
  //   {
  //     if (model is null) throw new System.ArgumentNullException(nameof(model));

  //     var client = new Client
  //     {
  //       Enabled = model.Enabled,
  //       ClientId = model.ClientId,
  //       ClientName = model.ClientName,
  //       RequireClientSecret = model.RequireClientSecret,
  //       RequirePkce = model.RequirePkce,
  //       RequireConsent = model.RequireConsent,
  //       AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
  //     };

  //     HandleCollectionProperties(model, client);

  //     this.context.Clients.Add(client);

  //     await this.context.SaveChangesAsync();

  //     return client.ClientId;
  //   }

  //   public async Task UpdateAsync(ClientViewModel model)
  //   {
  //     if (model == null) throw new ArgumentNullException(nameof(model));
  //     if (model.Id == null) throw new ArgumentNullException(nameof(model.Id));

  //     var client = await this.GetClientById(model.Id.Value);
  //     if (client == null) throw new ArgumentNullException(nameof(client));

  //     client.Enabled = model.Enabled;
  //     client.ClientId = model.ClientId;
  //     client.ClientName = model.ClientName;
  //     client.RequireClientSecret = model.RequireClientSecret;
  //     client.RequirePkce = model.RequirePkce;
  //     client.RequireConsent = model.RequireConsent;
  //     client.AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser;

  //     HandleCollectionProperties(model, client);

  //     this.context.Clients.Update(client);

  //     await this.context.SaveChangesAsync();
  //   }

  //   public async Task DeleteAsync(string clientId)
  //   {
  //     if (clientId == null) throw new ArgumentNullException(nameof(clientId));

  //     var client = await this.GetClientByClientId(clientId); ;

  //     this.context.Clients.Remove(client);

  //     await this.context.SaveChangesAsync();
  //   }

  //   private IOrderedQueryable<Client> LoadAll()
  //   {
  //     return this.context.Clients
  //             .Include(x => x.AllowedGrantTypes)
  //             .Include(x => x.RedirectUris)
  //             .Include(x => x.PostLogoutRedirectUris)
  //             .Include(x => x.AllowedCorsOrigins)
  //             .Include(x => x.AllowedScopes)
  //             .OrderBy(x => x.ClientName);
  //   }

  //   private async Task<Client> GetClientByClientId(string clientId)
  //   {
  //     List<Client> items = await this.LoadAll()
  //       .Where(x => x.ClientId == clientId)
  //       .ToListAsync();

  //     return items.Count() == 1 ? items.First() : null;
  //   }

  //   private async Task<Client> GetClientById(int id)
  //   {
  //     List<Client> items = await this.LoadAll()
  //       .Where(x => x.Id == id)
  //       .ToListAsync();

  //     return items.Count() == 1 ? items.First() : null;
  //   }

  //   private ClientViewModel ToModel(Client entity)
  //   {
  //     return new ClientViewModel
  //     {
  //       Id = entity.Id,
  //       Enabled = entity.Enabled,
  //       ClientId = entity.ClientId,
  //       ClientName = entity.ClientName,
  //       RequireClientSecret = entity.RequireClientSecret,
  //       RequirePkce = entity.RequirePkce,
  //       RequireConsent = entity.RequireConsent,
  //       AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
  //       ClientSecret = entity.ClientSecrets != null && entity.ClientSecrets.Count() > 0
  //         ? entity.ClientSecrets.FirstOrDefault().Value
  //         : null,
  //       AllowedGrantTypes = entity.AllowedGrantTypes
  //         .Select(x => x.GrantType).ToList(),
  //       RedirectUris = entity.RedirectUris
  //         .Select(x => x.RedirectUri).ToList(),
  //       PostLogoutRedirectUris = entity.PostLogoutRedirectUris
  //         .Select(x => x.PostLogoutRedirectUri).ToList(),
  //       AllowedCorsOrigins = entity.AllowedCorsOrigins
  //         .Select(x => x.Origin).ToList(),
  //       AllowedScopes = entity.AllowedScopes
  //         .Select(x => x.Scope).ToList()
  //     };
  //   }

  //   private static void HandleCollectionProperties(ClientViewModel model, Client client)
  //   {
  //     // deassign them
  //     if (client.AllowedGrantTypes != null) client.AllowedGrantTypes.Clear();
  //     if (client.RedirectUris != null) client.RedirectUris.Clear();
  //     if (client.PostLogoutRedirectUris != null) client.PostLogoutRedirectUris.Clear();
  //     if (client.AllowedCorsOrigins != null) client.AllowedCorsOrigins.Clear();
  //     if (client.AllowedScopes != null) client.AllowedScopes.Clear();
  //     if (client.ClientSecrets != null) client.ClientSecrets.Clear();

  //     // assign them
  //     client.AllowedGrantTypes = model.AllowedGrantTypes
  //       .Select(gt => new ClientGrantType
  //       {
  //         Client = client,
  //         GrantType = gt
  //       }).ToList();

  //     client.RedirectUris = model.RedirectUris
  //       .Select(u => new ClientRedirectUri
  //       {
  //         Client = client,
  //         RedirectUri = u
  //       }).ToList();

  //     client.PostLogoutRedirectUris = model.PostLogoutRedirectUris
  //       .Select(u => new ClientPostLogoutRedirectUri
  //       {
  //         Client = client,
  //         PostLogoutRedirectUri = u
  //       }).ToList();

  //     client.AllowedCorsOrigins = model.AllowedCorsOrigins
  //       .Select(o => new ClientCorsOrigin
  //       {
  //         Client = client,
  //         Origin = o
  //       }).ToList();

  //     client.AllowedScopes = model.AllowedScopes
  //       .Select(s => new ClientScope
  //       {
  //         Client = client,
  //         Scope = s
  //       }).ToList();

  //     if (model.ClientSecret != null)
  //     {
  //       client.ClientSecrets = new List<ClientSecret> {
  //         new ClientSecret {
  //           Client = client,
  //           Type = "tw",
  //           Value = model.ClientSecret
  //         }
  //       };
  //     }
  //   }
  // }
}