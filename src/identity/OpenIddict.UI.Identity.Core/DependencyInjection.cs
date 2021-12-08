using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Identity.Core
{
  [ExcludeFromCodeCoverage]
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
