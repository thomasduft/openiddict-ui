using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Identity.Api
{
  [ExcludeFromCodeCoverage]
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

      builder.Services.AddAuthorizationServices(options.Policy);

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

      return services;
    }

    private static IServiceCollection AddAuthorizationServices(
      this IServiceCollection services,
      Action<AuthorizationPolicyBuilder> policy
    )
    {
      services.AddAuthorization(options =>
      {
        options.AddPolicy(
          Policies.OPENIDDICT_UI_IDENTITY_API_POLICY,
          policy
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
        options.UseOpenIddictUIRoutePrefix(routePrefix, new List<Type>
        {
          typeof(AccountController),
          typeof(ClaimTypeController),
          typeof(RoleController)
        });
      });

      return builder;
    }
  }
}