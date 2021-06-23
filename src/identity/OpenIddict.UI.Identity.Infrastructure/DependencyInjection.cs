using Microsoft.Extensions.DependencyInjection;
using tomware.OpenIddict.UI.Core;
using tomware.OpenIddict.UI.Identity.Core;

namespace tomware.OpenIddict.UI.Identity.Infrastructure
{
  public static class OpenIddictUICoreServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUIIdentityInfrastructureServices(
      this IServiceCollection services
    )
    {
      services.AddOpenIddictUIIdentityCoreServices();

      services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

      services.AddTransient<IClaimTypeRepository, ClaimTypeRepository<OpenIddictUIIdentityContext>>();

      return services;
    }
  }
}
