using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Core
{
  public static class OpenIddictUICoreServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUICoreServices(
      this IServiceCollection services
    )
    {
      services.AddTransient<IClaimTypeService, ClaimTypeService>();

      return services;
    }
  }
}
