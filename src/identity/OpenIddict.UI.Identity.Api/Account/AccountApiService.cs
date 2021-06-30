using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public interface IAccountApiService
  {
    Task<IdentityResult> RegisterAsync(RegisterUserViewModel model);
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
    Task<UserViewModel> GetUserAsync(string id);
    Task<IdentityResult> UpdateAsync(UserViewModel model);
    Task<IdentityResult> DeleteAsync(string id);
  }

  public class AccountApiService<TIdentityUser> : IAccountApiService
    where TIdentityUser : IdentityUser, new()
  {
    // TODO: move UserManager down to Infrastructure
    // using Microsoft.EntityFrameworkCore is wrong in the Api layer!
    // remove package dependency <PackageReference Include="OpenIddict.EntityFrameworkCore" Version="3.0.5" />
    private readonly UserManager<TIdentityUser> _manager;
    private readonly IUserCreationStrategy<TIdentityUser> _userCreationStrategy;

    public AccountApiService(
      UserManager<TIdentityUser> manager,
      IUserCreationStrategy<TIdentityUser> userCreationStrategy
    )
    {
      _manager = manager
        ?? throw new ArgumentNullException(nameof(manager));
      _userCreationStrategy = userCreationStrategy
        ?? throw new ArgumentNullException(nameof(userCreationStrategy));
    }

    public async Task<IdentityResult> RegisterAsync(
      RegisterUserViewModel model
    )
    {
      var identiyUser = _userCreationStrategy.CreateUser(model);

      return await _manager.CreateAsync(identiyUser, model.Password);
    }

    public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
    {
      // TODO: Paging ???
      var items = await _manager.Users
        .OrderBy(u => u.UserName)
        .AsNoTracking()
        .ToListAsync();

      return items.Select(u => new UserViewModel
      {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        LockoutEnabled = u.LockoutEnabled
      });
    }

    public async Task<UserViewModel> GetUserAsync(string id)
    {
      var user = await _manager.FindByIdAsync(id);
      var roles = await _manager.GetRolesAsync(user);
      var claims = await _manager.GetClaimsAsync(user);

      var isLockedOut = await _manager.IsLockedOutAsync(user);

      return new UserViewModel
      {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        LockoutEnabled = user.LockoutEnabled,
        IsLockedOut = isLockedOut,
        Claims = new List<ClaimViewModel>(claims.ToList().Select(x => new ClaimViewModel
        {
          Type = x.Type,
          Value = x.Value
        })),
        Roles = roles.ToList()
      };
    }

    public async Task<IdentityResult> UpdateAsync(UserViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));
      if (string.IsNullOrWhiteSpace(model.Id))
        throw new InvalidOperationException(nameof(model.Id));

      var user = await _manager.FindByIdAsync(model.Id);
      user.UserName = model.UserName;
      user.Email = model.Email;
      user.LockoutEnabled = model.LockoutEnabled;

      var result = await _manager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        return result;
      }

      result = await AssignClaimsAsync(
        user,
        model.Claims.Select(x => new Claim(x.Type, x.Value)).ToList()
      );
      if (!result.Succeeded)
      {
        return result;
      }

      result = await AssignRolesAsync(user, model.Roles);
      if (!result.Succeeded)
      {
        return result;
      }

      return result;
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException(nameof(id));

      var user = await _manager.FindByIdAsync(id);

      return await _manager.DeleteAsync(user);
    }

    private async Task<IdentityResult> AssignClaimsAsync(
      TIdentityUser user,
      IEnumerable<Claim> claims
    )
    {
      // removing all claims
      var existingClaims = await _manager.GetClaimsAsync(user);
      await _manager.RemoveClaimsAsync(user, existingClaims);

      // assigning claims
      return await _manager.AddClaimsAsync(
        user,
        claims
      );
    }

    private async Task<IdentityResult> AssignRolesAsync(
      TIdentityUser user,
      IEnumerable<string> roles
    )
    {
      // removing all roles
      var existingRoles = await _manager.GetRolesAsync(user);
      await _manager.RemoveFromRolesAsync(user, existingRoles);

      // assigning roles
      return await _manager.AddToRolesAsync(
        user,
        roles
      );
    }
  }
}
