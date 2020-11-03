using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI
{
  public static class OpenIddictUIServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUIServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser
    {
      services.AddAuthorizationServices();

      services.AddApiServices<TApplicationUser>();

      return services;
    }

    private static IServiceCollection AddAuthorizationServices(
      this IServiceCollection services
    )
    {
      services.AddAuthorization(options =>
      {
        options.AddPolicy(
          Policies.ADMIN_POLICY,
          policy => policy
            .RequireAuthenticatedUser()
            .RequireRole(Roles.ADMINISTRATOR_ROLE)
        );
      });

      return services;
    }

    private static IServiceCollection AddApiServices<T>(
      this IServiceCollection services
    ) where T : IdentityUser
    {
      services.AddTransient<IAccountService, AccountService<T>>();
      services.AddTransient<IRoleService, RoleService>();

      // TODO: with OpenIddict managers
      // services.AddTransient<IClientService, ClientService>();
      // services.AddTransient<IScopeService, ScopeService>();

      // TODO: anyway custom implementation!
      //services.AddTransient<IClaimTypeService, ClaimTypeService>();

      return services;
    }
  }
}