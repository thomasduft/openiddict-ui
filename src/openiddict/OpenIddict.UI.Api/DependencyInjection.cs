using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Api;

[ExcludeFromCodeCoverage]
public static class OpenIddictUIServicesExtensions
{
  /// <summary>
  /// Register the Api for the EF based UI Store.
  /// </summary>
  public static OpenIddictBuilder AddUIApis(
    this OpenIddictBuilder builder,
    Action<OpenIddictUIApiOptions> uiApiOptions = null
  )
  {
    var options = new OpenIddictUIApiOptions();
    uiApiOptions?.Invoke(options);
    builder.AddRoutePrefix(options.RoutePrefix);

    builder.Services.AddApiServices(uiApiOptions);

    builder.Services.AddAuthorizationServices(options.Policy);

    return builder;
  }

  private static IServiceCollection AddApiServices(
    this IServiceCollection services,
    Action<OpenIddictUIApiOptions> options = null
  )
  {
    services.Configure(options);

    services.AddTransient<IScopeApiService, ScopeApiService>();
    services.AddTransient<IApplicationApiService, ApplicationApiService>();

    return services;
  }

  private static IServiceCollection AddAuthorizationServices(
    this IServiceCollection services,
    Action<AuthorizationPolicyBuilder> policy
  )
  {
    services.AddAuthorizationBuilder()
        .AddPolicy(Policies.OpenIddictUiApiPolicy, policy
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
          typeof(ApplicationController),
          typeof(ScopeController)
        ]
      );
    });

    return builder;
  }
}
