using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.Microip.Web
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync();
  }

  public class MigrationService : IMigrationService
  {
    private readonly STSContext context;
    private readonly OpenIddictUIContext openIddictUIContext;
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration config;

    public MigrationService(
      STSContext context,
      OpenIddictUIContext openopenIddictUIContext,
      OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IConfiguration config
      )
    {
      this.context = context;
      this.openIddictUIContext = openopenIddictUIContext;
      this.applicationManager = applicationManager;
      this.userManager = userManager;
      this.roleManager = roleManager;
      this.config = config;
    }

    public async Task EnsureMigrationAsync()
    {
      await this.context.Database.MigrateAsync();
      await this.openIddictUIContext.Database.MigrateAsync();

      await this.EnsureAdministratorRole();
      await this.EnsureAdministratorUser();

      var authority = !string.IsNullOrWhiteSpace(this.config["AuthorityForDocker"])
        ? this.config["AuthorityForDocker"]
        : Program.GetUrls(this.config);

      await this.EnsureAdminApplication(authority);
    }

    private async Task EnsureAdministratorRole()
    {
      var role = Roles.ADMINISTRATOR_ROLE;
      var roleExists = await this.roleManager.RoleExistsAsync(role);
      if (!roleExists)
      {
        var newRole = new IdentityRole(role);
        await this.roleManager.CreateAsync(newRole);
      }
    }

    private async Task EnsureAdministratorUser()
    {
      var user = await this.userManager.FindByNameAsync(Constants.ADMIN_USER);
      if (user != null) return;

      var applicationUser = new ApplicationUser
      {
        UserName = Constants.ADMIN_USER,
        Email = "admin@sts.com"
      };

      var userResult = await this.userManager.CreateAsync(applicationUser, "Pass123$");
      if (!userResult.Succeeded) return;

      await this.userManager.SetLockoutEnabledAsync(applicationUser, false);
      await this.userManager.AddToRoleAsync(applicationUser, Roles.ADMINISTRATOR_ROLE);
    }

    private async Task EnsureAdminApplication(string authority)
    {
      if (await this.applicationManager.FindByClientIdAsync("stsclient") == null)
      {
        await this.applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
          ClientId = "stsclient",
          DisplayName = "STS Admin UI",
          // ClientSecret = "",
          ConsentType = ConsentTypes.Implicit,
          Requirements =
          {
            Requirements.Features.ProofKeyForCodeExchange
          },
          RedirectUris =
          {
            new Uri(authority),
            new Uri("http://localhost:4200")
          },
          PostLogoutRedirectUris =
          {
            new Uri(authority),
            new Uri("http://localhost:4200")
          },
          Permissions =
          {
            Permissions.ResponseTypes.Code,
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Logout,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles,
            Permissions.Prefixes.Scope + Constants.STS_API
          }
        });
      }
    }
  }
}