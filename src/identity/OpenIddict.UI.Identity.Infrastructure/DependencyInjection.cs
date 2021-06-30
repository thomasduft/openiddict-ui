using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Core;
using tomware.OpenIddict.UI.Identity.Core;

namespace tomware.OpenIddict.UI.Identity.Infrastructure
{
  public static class OpenIddictUIIdentityInfrastructureServicesExtensions
  {
    // TODO: better naming
    public static OpenIddictBuilder AddUIIdentityStore(
      this OpenIddictBuilder builder,
      Action<OpenIddictUIIdentityStoreOptions> storeOptionsAction = null
    )
    {
      builder.Services.AddOpenIddictUIIdentityInfrastructureServices();

      var coreBuilder = new OpenIddictCoreBuilder(builder.Services)
        .UseEFCoreUIStore<OpenIddictUIIdentityContext>();

      builder.Services
        .AddOpenIddictUIIdentityStore<OpenIddictUIIdentityContext>(storeOptionsAction);

      return builder;
    }

    private static IServiceCollection AddOpenIddictUIIdentityInfrastructureServices(
      this IServiceCollection services
    )
    {
      services.AddOpenIddictUIIdentityCoreServices();

      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIIdentityContext>>();

      return services;
    }

    private static OpenIddictEntityFrameworkCoreBuilder UseEFCoreUIStore<TContext>(
      this OpenIddictCoreBuilder builder
    ) where TContext : IOpenIddictUIIdentityContext
    {
      builder
        .UseEntityFrameworkCore()
        .UseDbContext(typeof(TContext));

      return new OpenIddictEntityFrameworkCoreBuilder(builder.Services);
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
