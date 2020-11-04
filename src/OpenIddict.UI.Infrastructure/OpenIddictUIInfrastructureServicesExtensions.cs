using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public static class OpenIddictUIInfrastructureServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUIInfrastructureServices(
      this IServiceCollection services
    )
    {
      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

      services.AddScoped<IMigrationService, MigrationService>();

      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIContext>>();
      services.AddTransient<IScopeRepository, ScopeRepository<OpenIddictUIContext>>();

      return services;
    }
  }
}
