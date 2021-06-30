using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public static class OpenIddictUIIdentityCoreServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUIIdentityCoreServices(
      this IServiceCollection services
    )
    {
      services.AddTransient<IClaimTypeService, ClaimTypeService>();

      return services;
    }
  }
}
