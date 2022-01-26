using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public interface IUserCreationStrategy<TIdentityUser>
    where TIdentityUser : IdentityUser
  {
    TIdentityUser CreateUser(RegisterUserParam model);
  }
}