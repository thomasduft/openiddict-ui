using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Identity.Core;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Identity.Api;

[ExcludeFromCodeCoverage]
public static class OpenIddictUIIdentityApiServicesExtensions
{
  public static OpenIddictBuilder AddUIIdentityApis<TApplicationUser>(
    this OpenIddictBuilder builder,
    Action<OpenIddictUIIdentityApiOptions> uiApiOptions = null
  )
    where TApplicationUser : IdentityUser, new() => AddUIIdentityApis<TApplicationUser, IdentityRole, string>(builder, uiApiOptions);

  public static OpenIddictBuilder AddUIIdentityApis<TApplicationUser, TApplicationRole, TKey>(
    this OpenIddictBuilder builder,
    Action<OpenIddictUIIdentityApiOptions> uiApiOptions = null
  )
    where TKey : IEquatable<TKey>
    where TApplicationUser : IdentityUser<TKey>, new()
    where TApplicationRole : IdentityRole<TKey>, new()
  {
    var options = new OpenIddictUIIdentityApiOptions();
    uiApiOptions?.Invoke(options);
    builder.AddRoutePrefix(options.RoutePrefix);

    builder.Services.AddApiServices<TApplicationUser, TApplicationRole, TKey>();

    builder.Services.AddAuthorizationServices(options.Policy);

    return builder;
  }

  /// <summary>
  /// Registers the UserName to UserName UserCreationStrategy.
  /// </summary>
  public static OpenIddictBuilder AddUserNameUserCreationStrategy<TApplicationUser, TKey>(
    this OpenIddictBuilder builder
  )
    where TKey : IEquatable<TKey>
    where TApplicationUser : IdentityUser<TKey>, new()
  {
    builder.Services
      .AddUserNameUserCreationStrategy<TApplicationUser, TKey>();

    return builder;
  }

  private static IServiceCollection AddApiServices<TApplicationUser, TApplicationRole, TKey>(
    this IServiceCollection services
  )
    where TKey : IEquatable<TKey>
    where TApplicationUser : IdentityUser<TKey>, new()
    where TApplicationRole : IdentityRole<TKey>, new()
  {
    services.AddTransient<IAccountApiService, AccountApiService<TApplicationUser, TKey>>();
    services.AddTransient<IRoleApiService, RoleApiService<TApplicationRole, TKey>>();
    services.AddTransient<IClaimTypeApiService, ClaimTypeApiService>();

    return services;
  }

  private static IServiceCollection AddAuthorizationServices(
    this IServiceCollection services,
    Action<AuthorizationPolicyBuilder> policy
  )
  {
    services.AddAuthorizationBuilder()
        .AddPolicy(Policies.OpenIddictUiIdentityApiPolicy, policy
);

    return services;
  }

  private static OpenIddictBuilder AddRoutePrefix(
    this OpenIddictBuilder builder,
    string routePrefix
  )
  {
    builder.Services.AddControllers(options =>
    {
      options.UseOpenIddictUIRoutePrefix(
        routePrefix,
        [
          typeof(AccountController),
          typeof(ClaimTypeController),
          typeof(RoleController)
        ]
      );
    });

    return builder;
  }
}
