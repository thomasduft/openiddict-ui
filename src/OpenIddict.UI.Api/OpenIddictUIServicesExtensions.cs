using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Api
{
  public static class OpenIddictUIServicesExtensions
  {
    public static OpenIddictBuilder AddUIApis<TApplicationUser>(
      this OpenIddictBuilder builder,
      OpenIddictUIApiOptions apiOptionsAction
    ) where TApplicationUser : IdentityUser, new()
    {
      builder.Services
        .AddOpenIddictUIApiServices<TApplicationUser>(apiOptionsAction);

      return builder;
    }

    private static IServiceCollection AddOpenIddictUIApiServices<TApplicationUser>(
      this IServiceCollection services,
      OpenIddictUIApiOptions apiOptionsAction
    ) where TApplicationUser : IdentityUser, new()
    {
      services.AddAuthorizationServices();

      services.AddApiServices<TApplicationUser>();

      services.Configure<OpenIddictUIApiOptions>(options => {
        options.Permissions = apiOptionsAction.Permissions;
        options.Requirements = apiOptionsAction.Requirements;
      });

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

    private static IServiceCollection AddApiServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      // Identity services
      services.AddTransient<IAccountApiService, AccountApiService<TApplicationUser>>();
      services.AddTransient<IRoleService, RoleService>();
      services.AddTransient<IClaimTypeApiService, ClaimTypeApiService>();

      // OpenIddict services
      services.AddTransient<IScopeApiService, ScopeApiService>();
      services.AddTransient<IApplicationApiService, ApplicationApiService>();

      return services;
    }
  }
}