using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Suite.Core;
using tomware.OpenIddict.UI.Identity.Core;

namespace tomware.OpenIddict.UI.Identity.Infrastructure
{
  public static class OpenIddictUIIdentityInfrastructureServicesExtensions
  {
    public static OpenIddictBuilder AddUIIdentityStore(
      this OpenIddictBuilder builder,
      Action<OpenIddictUIIdentityStoreOptions> storeOptionsAction = null
    )
    {
      builder.Services.AddInfrastructureServices();

      builder.Services
        .AddOpenIddictUIIdentityStore<OpenIddictUIIdentityContext>(storeOptionsAction);

      return builder;
    }

    private static IServiceCollection AddInfrastructureServices(
      this IServiceCollection services
    )
    {
      services.AddOpenIddictUIIdentityCoreServices();

      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIIdentityContext>>();

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
