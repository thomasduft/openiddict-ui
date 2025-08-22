using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Infrastructure;

[ExcludeFromCodeCoverage]
public static class OpenIddictUIInfrastructureServicesExtensions
{
  public static OpenIddictBuilder AddUIStore(
    this OpenIddictBuilder builder,
    Action<OpenIddictUIStoreOptions> storeOptionsAction = null
  )
  {
    builder.Services.AddInfrastructureServices();

    var coreBuilder = new OpenIddictCoreBuilder(builder.Services)
      .UseEFCoreUIStore<OpenIddictUIContext>();

    builder.Services
      .AddOpenIddictUIStore<OpenIddictUIContext>(storeOptionsAction);

    return builder;
  }

  private static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services
  )
  {
    services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

    services.AddTransient<IScopeRepository, ScopeRepository<OpenIddictUIContext>>();
    services.AddTransient<IApplicationRepository, ApplicationRepository<OpenIddictUIContext>>();

    services.AddTransient<IScopeService, ScopeService>();
    services.AddTransient<IApplicationService, ApplicationService>();

    return services;
  }

  private static OpenIddictEntityFrameworkCoreBuilder UseEFCoreUIStore<TContext>(
    this OpenIddictCoreBuilder builder
  ) where TContext : IOpenIddictUIContext
  {
    builder
      .UseEntityFrameworkCore()
      .UseDbContext<OpenIddictUIContext>();

    return new OpenIddictEntityFrameworkCoreBuilder(builder.Services);
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
