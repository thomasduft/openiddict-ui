using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using tomware.OpenIddict.UI.Suite.Core;
using tomware.OpenIddict.UI.Identity.Core;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Infrastructure
{
  [ExcludeFromCodeCoverage]
  public static class OpenIddictUIIdentityInfrastructureServicesExtensions
  {
    public static OpenIddictBuilder AddUIIdentityStore<TApplicationUser>(
      this OpenIddictBuilder builder,
      Action<OpenIddictUIIdentityStoreOptions> storeOptionsAction = null
    ) where TApplicationUser : IdentityUser, new()
    {
      builder.Services.AddInfrastructureServices<TApplicationUser>();

      builder.Services
        .AddOpenIddictUIIdentityStore<OpenIddictUIIdentityContext>(storeOptionsAction);

      return builder;
    }

    private static IServiceCollection AddInfrastructureServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      // core services
      services.AddOpenIddictUIIdentityCoreServices<TApplicationUser>();

      // own services
      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));
      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIIdentityContext>>();
      services.AddTransient<IAccountService, AccountService<TApplicationUser>>();

      return services;
    }

    private static IServiceCollection AddOpenIddictUIIdentityStore<TContext>(
      this IServiceCollection services,
      Action<OpenIddictUIIdentityStoreOptions> storeOptionsAction = null)
      where TContext : DbContext, IOpenIddictUIIdentityContext
    {
      var options = new OpenIddictUIIdentityStoreOptions();
      services.AddSingleton(options);
      storeOptionsAction?.Invoke(options);

      if (options.ResolveDbContextOptions != null)
      {
        services.AddDbContext<TContext>(options.ResolveDbContextOptions);
      }
      else
      {
        services.AddDbContext<TContext>(dbCtxBuilder =>
        {
          options.OpenIddictUIIdentityContext?.Invoke(dbCtxBuilder);
        });
      }

      services.AddScoped<IOpenIddictUIIdentityContext, TContext>();

      return services;
    }
  }
}
