using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvc.Server.Helpers;
using Mvc.Server.Models;
using OpenIddict.Abstractions;
using tomware.OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Mvc.Server
{
  public class Worker : IHostedService
  {
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      using var scope = _serviceProvider.CreateScope();

      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      await context.Database.MigrateAsync(cancellationToken);

      var uIContext = scope.ServiceProvider.GetRequiredService<OpenIddictUIContext>();
      await uIContext.Database.MigrateAsync(cancellationToken);

      await RegisterApplicationsAsync(scope.ServiceProvider);
      await RegisterScopesAsync(scope.ServiceProvider);

      await EnsureAdministratorRole(scope.ServiceProvider);
      await EnsureAdministratorUser(scope.ServiceProvider);

      static async Task RegisterApplicationsAsync(IServiceProvider provider)
      {
        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("spa_client") is null)
        {
          await manager.CreateAsync(new OpenIddictApplicationDescriptor
          {
            ClientId = "spa_client",
            // ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "SPA Client Application",
            DisplayNames =
            {
              [CultureInfo.GetCultureInfo("fr-FR")] = "Application cliente SPA"
            },
            PostLogoutRedirectUris =
            {
              new Uri("https://localhost:5000/signout-callback-oidc"),
              new Uri("http://localhost:4200")
            },
            RedirectUris =
            {
              new Uri("https://localhost:5000/signin-oidc"),
              new Uri("http://localhost:4200")
            },
            Permissions =
            {
              Permissions.Endpoints.Authorization,
              Permissions.Endpoints.Logout,
              Permissions.Endpoints.Token,
              Permissions.GrantTypes.AuthorizationCode,
              Permissions.GrantTypes.RefreshToken,
              Permissions.ResponseTypes.Code,
              Permissions.Scopes.Email,
              Permissions.Scopes.Profile,
              Permissions.Scopes.Roles,
              Permissions.Prefixes.Scope + "demo_api"
            },
            Requirements =
            {
              Requirements.Features.ProofKeyForCodeExchange
            }
          });
        }

        // To test this sample with Postman, use the following settings:
        //
        // * Authorization URL: https://localhost:5000/connect/authorize
        // * Access token URL: https://localhost:5000/connect/token
        // * Client ID: postman
        // * Client secret: [blank] (not used with public clients)
        // * Scope: openid email profile roles
        // * Grant type: authorization_code
        // * Request access token locally: yes
        if (await manager.FindByClientIdAsync("postman") is null)
        {
          await manager.CreateAsync(new OpenIddictApplicationDescriptor
          {
            ClientId = "postman",
            ConsentType = ConsentTypes.Systematic,
            DisplayName = "Postman",
            RedirectUris =
            {
              new Uri("urn:postman")
            },
            Permissions =
            {
              Permissions.Endpoints.Authorization,
              Permissions.Endpoints.Device,
              Permissions.Endpoints.Token,
              Permissions.GrantTypes.AuthorizationCode,
              Permissions.GrantTypes.DeviceCode,
              Permissions.GrantTypes.Password,
              Permissions.GrantTypes.RefreshToken,
              Permissions.ResponseTypes.Code,
              Permissions.Scopes.Email,
              Permissions.Scopes.Profile,
              Permissions.Scopes.Roles
            }
          });
        }

        // To test this sample with Postman, use the following settings:
        //
        // * Access token URL: https://localhost:5000/connect/token
        // * grant_type: password
        // * client_id: test-client
        // * client_secret: my-secret
        // * username: admin@openiddict.com
        // * password: Pass123$
        // * scope: openid email profile roles
        // * Request access token locally: yes
        if (await manager.FindByClientIdAsync("test-client") is null)
        {
          await manager.CreateAsync(new OpenIddictApplicationDescriptor
          {
            ClientId = "test-client",
            ConsentType = ConsentTypes.Systematic,
            DisplayName = "Tester",
            ClientSecret = "my-secret",
            Permissions =
            {
              Permissions.Endpoints.Authorization,
              Permissions.Endpoints.Token,
              Permissions.GrantTypes.Password,
              Permissions.GrantTypes.RefreshToken,
              Permissions.ResponseTypes.Token,
              Permissions.Scopes.Email,
              Permissions.Scopes.Profile,
              Permissions.Scopes.Roles
            }
          });
        }
      }

      static async Task RegisterScopesAsync(IServiceProvider provider)
      {
        var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

        if (await manager.FindByNameAsync("demo_api") is null)
        {
          await manager.CreateAsync(new OpenIddictScopeDescriptor
          {
            DisplayName = "Demo API access",
            DisplayNames =
            {
              [CultureInfo.GetCultureInfo("fr-FR")] = "Accès à l'API de démo"
            },
            Name = "demo_api",
            Resources =
            {
              "resource_server"
            }
          });
        }
      }

      static async Task EnsureAdministratorRole(IServiceProvider provider)
      {
        var manager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        var role = Roles.ADMINISTRATOR_ROLE;
        var roleExists = await manager.RoleExistsAsync(role);
        if (!roleExists)
        {
          var newRole = new IdentityRole(role);
          await manager.CreateAsync(newRole);
        }
      }

      static async Task EnsureAdministratorUser(IServiceProvider provider)
      {
        var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await manager.FindByNameAsync(Constants.ADMIN_MAILADDRESS);
        if (user != null) return;

        var applicationUser = new ApplicationUser
        {
          UserName = Constants.ADMIN_MAILADDRESS,
          Email = Constants.ADMIN_MAILADDRESS
        };

        var userResult = await manager.CreateAsync(applicationUser, "Pass123$");
        if (!userResult.Succeeded) return;

        await manager.SetLockoutEnabledAsync(applicationUser, false);
        await manager.AddToRoleAsync(applicationUser, Roles.ADMINISTRATOR_ROLE);
      }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
