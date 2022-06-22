using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Identity.Core;

[ExcludeFromCodeCoverage]
public static class OpenIddictUIIdentityCoreServicesExtensions
{
  public static IServiceCollection AddOpenIddictUIIdentityCoreServices<TApplicationUser, TKey>(
    this IServiceCollection services
  )
    where TKey : IEquatable<TKey>
    where TApplicationUser : IdentityUser<TKey>, new()
  {
    services.AddTransient<IClaimTypeService, ClaimTypeService>();
    services.AddTransient<IUserCreationStrategy<TApplicationUser, TKey>, EmailUserCreationStrategy<TApplicationUser, TKey>>();

    return services;
  }

  /// <summary>
  /// Registers the UserName to UserName UserCreationStrategy.
  /// </summary>
  public static IServiceCollection AddUserNameUserCreationStrategy<TApplicationUser, TKey>(
    this IServiceCollection services
  )
    where TKey : IEquatable<TKey>
    where TApplicationUser : IdentityUser<TKey>, new()
  {
    services
      .AddTransient<IUserCreationStrategy<TApplicationUser, TKey>, UserNameUserCreationStrategy<TApplicationUser, TKey>>();

    return services;
  }
}
