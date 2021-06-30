using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public interface IUserCreationStrategy<TIdentityUser>
    where TIdentityUser : IdentityUser
  {
    TIdentityUser CreateUser(RegisterUserViewModel model);
  }

  /// <summary>
  /// <para>Maps the Email property to the IdentityUser.UserName property.</para>
  /// <para>Note: If you use the ASP.NET Core Identity UI this will be the default.</para>
  /// <para>Weird to me ?!</para>
  /// </summary>
  public class EmailUserCreationStrategy<TIdentityUser>
    : IUserCreationStrategy<TIdentityUser>
    where TIdentityUser : IdentityUser, new()
  {
    public TIdentityUser CreateUser(RegisterUserViewModel model)
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
  public class UserNameUserCreationStrategy<TIdentityUser>
    : IUserCreationStrategy<TIdentityUser>
    where TIdentityUser : IdentityUser, new()
  {
    public TIdentityUser CreateUser(RegisterUserViewModel model)
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