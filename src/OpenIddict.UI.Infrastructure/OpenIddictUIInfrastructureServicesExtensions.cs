using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public static class OpenIddictUIInfrastructureServicesExtensions
  {
    public static OpenIddictBuilder AddUIStore(
      this OpenIddictBuilder builder,
      Action<OpenIddictUIStoreOptions> storeOptionsAction = null
    )
    {
      builder.Services.AddOpenIddictUICoreServices();
      builder.Services.AddOpenIddictUIInfrastructureServices();

      var coreBuilder = new OpenIddictCoreBuilder(builder.Services)
        .UseEFCoreUIStore<OpenIddictUIContext>();
      
      builder.Services.AddOpenIddictUIStore<OpenIddictUIContext>(storeOptionsAction);

      return builder;
    }

    private static OpenIddictEntityFrameworkCoreBuilder UseEFCoreUIStore<TContext>(
      this OpenIddictCoreBuilder builder
    ) where TContext : IOpenIddictUIContext
    {
      builder
        .UseEntityFrameworkCore()
        .UseDbContext(typeof(TContext));

      return new OpenIddictEntityFrameworkCoreBuilder(builder.Services);
    }

    private static IServiceCollection AddOpenIddictUIInfrastructureServices(
      this IServiceCollection services
    )
    {
      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIContext>>();
      services.AddTransient<IScopeRepository, ScopeRepository<OpenIddictUIContext>>();

      return services;
    }

    private static IServiceCollection AddOpenIddictUIStore<TContext>(
      this IServiceCollection services,
      Action<OpenIddictUIStoreOptions> storeOptionsAction = null)
      where TContext : DbContext, IOpenIddictUIContext
    {
      var options = new OpenIddictUIStoreOptions();
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
          options.OpenIddictUIContext?.Invoke(dbCtxBuilder);
        });
      }

      services.AddScoped<IOpenIddictUIContext, TContext>();

      return services;
    }
  }
}
