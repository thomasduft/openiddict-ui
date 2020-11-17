using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI.Api
{
  public interface IAccountApiService
  {
    Task<IdentityResult> RegisterAsync(RegisterUserViewModel model);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
    Task<UserViewModel> GetUserAsync(string id);
    Task<IdentityResult> UpdateAsync(UserViewModel model);
  }

  public class AccountApiService<TIdentityUser> : IAccountApiService
    where TIdentityUser : IdentityUser, new()
  {
    private readonly UserManager<TIdentityUser> _manager;

    public AccountApiService(
      UserManager<TIdentityUser> manager
    )
    {
      _manager = manager;
    }

    public async Task<IdentityResult> RegisterAsync(
      RegisterUserViewModel model
    )
    {
      // TODO: provide strategy for enabling correct UserName/Email login
      // for not misusing the Email as UserName!
      var identiyUser = new TIdentityUser
      {
        UserName = model.Email, // model.UserName
        Email = model.Email,
        LockoutEnabled = true
      };

      return await _manager.CreateAsync(identiyUser, model.Password);
    }

    public async Task<IdentityResult> ChangePasswordAsync(
      ChangePasswordViewModel model
    )
    {
      var user = await _manager.FindByNameAsync(model.UserName);

      return await _manager.ChangePasswordAsync(
        user,
        model.CurrentPassword,
        model.NewPassword
      );
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
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

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
