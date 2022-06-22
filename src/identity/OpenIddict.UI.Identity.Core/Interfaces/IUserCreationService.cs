using System;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Core;

public interface IUserCreationStrategy<TIdentityUser, TKey>
  where TKey : IEquatable<TKey>
  where TIdentityUser : IdentityUser<TKey>
{
  TIdentityUser CreateUser(RegisterUserParam model);
}
