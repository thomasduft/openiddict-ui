using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public static class OpenIddictUIIdentityApiServicesExtensions
  {
    public static OpenIddictBuilder AddUIIdentityApis<TApplicationUser>(
      this OpenIddictBuilder builder,
      Action<OpenIddictUIIdentityApiOptions> uiApiOptions = null
    ) where TApplicationUser : IdentityUser, new()
    {
      var options = new OpenIddictUIIdentityApiOptions();
      uiApiOptions?.Invoke(options);
      builder.AddRoutePrefix(options.RoutePrefix);

      builder.Services.AddApiServices<TApplicationUser>();

      return builder;
    }

    /// <summary>
    /// Registers the UserName to UserName UserCreationStrategy.
    /// </summary>
    public static OpenIddictBuilder AddUserNameUserCreationStrategy<TApplicationUser>(
      this OpenIddictBuilder builder
    ) where TApplicationUser : IdentityUser, new()
    {
      builder.Services
        .AddTransient<IUserCreationStrategy<TApplicationUser>, UserNameUserCreationStrategy<TApplicationUser>>();

      return builder;
    }

    private static IServiceCollection AddApiServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      services.AddTransient<IUserCreationStrategy<TApplicationUser>, EmailUserCreationStrategy<TApplicationUser>>();
      services.AddTransient<IAccountApiService, AccountApiService<TApplicationUser>>();
      services.AddTransient<IRoleService, RoleService>();
      services.AddTransient<IClaimTypeApiService, ClaimTypeApiService>();

      services.AddAuthorizationServices();

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
    
    private static OpenIddictBuilder AddRoutePrefix(
      this OpenIddictBuilder builder,
      string routePrefix
    )
    {
      builder.Services.AddControllers(options =>
      {
        options.UseOpenIddictUIIdentityRoutePrefix(routePrefix);
      });

      return builder;
    }
  }
}