using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Suite.Core;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Api
{
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
        options.UseOpenIddictUIRoutePrefix(routePrefix, new List<Type>
        {
          typeof(ApplicationController),
          typeof(ScopeController)
        });
      });

      return builder;
    }
  }
}