using System;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Core
{
  /// <summary>
  /// <para>Maps the Email property to the IdentityUser.UserName property.</para>
  /// <para>Note: If you use the ASP.NET Core Identity UI this will be the default.</para>
  /// <para>Weird to me ?!</para>
  /// </summary>
  public class EmailUserCreationStrategy<TIdentityUser, TKey>
    : IUserCreationStrategy<TIdentityUser, TKey>
    where TKey : IEquatable<TKey>
    where TIdentityUser : IdentityUser<TKey>, new()
  {
    public TIdentityUser CreateUser(RegisterUserParam model)
    {
      return new TIdentityUser
      {
        UserName = model.Email, // Default ASP.NET Identity UI misuses the UserName
        Email = model.Email,
        LockoutEnabled = true
      };
    }
  }

  /// <summary>
  /// Maps the UserName property to the IdentityUser.UserName property.
  /// </summary>
  public class UserNameUserCreationStrategy<TIdentityUser, TKey>
    : IUserCreationStrategy<TIdentityUser, TKey>
    where TKey : IEquatable<TKey>
    where TIdentityUser : IdentityUser<TKey>, new()
  {
    public TIdentityUser CreateUser(RegisterUserParam model)
    {
      return new TIdentityUser
      {
        UserName = model.UserName,
        Email = model.Email,
        LockoutEnabled = true
      };
    }
  }
}