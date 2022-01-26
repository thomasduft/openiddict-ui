using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Identity.Core
{
  [ExcludeFromCodeCoverage]
  public static class OpenIddictUIIdentityCoreServicesExtensions
  {
    public static IServiceCollection AddOpenIddictUIIdentityCoreServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      services.AddTransient<IClaimTypeService, ClaimTypeService>();
      services.AddTransient<IUserCreationStrategy<TApplicationUser>, EmailUserCreationStrategy<TApplicationUser>>();

      return services;
    }

    /// <summary>
    /// Registers the UserName to UserName UserCreationStrategy.
    /// </summary>
    public static IServiceCollection AddUserNameUserCreationStrategy<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      services
        .AddTransient<IUserCreationStrategy<TApplicationUser>, UserNameUserCreationStrategy<TApplicationUser>>();

      return services;
    }
  }
}
