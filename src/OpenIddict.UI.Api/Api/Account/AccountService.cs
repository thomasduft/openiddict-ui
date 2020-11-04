using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI.Api
{
  public interface IAccountService
  {
    IdentityUser CreateUser(RegisterUserViewModel model);
    Task<IdentityResult> RegisterAsync(IdentityUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
    Task<UserViewModel> GetUserAsync(string id);
    Task<IdentityResult> UpdateAsync(UserViewModel model);
  }

  public class AccountService<TIdentityUser> : IAccountService
    where TIdentityUser : IdentityUser
  {
    private readonly UserManager<TIdentityUser> manager;

    public AccountService(
      UserManager<TIdentityUser> manager
    )
    {
      this.manager = manager;
    }

    public IdentityUser CreateUser(
      RegisterUserViewModel model
    )
    {
      return new IdentityUser
      {
        UserName = model.UserName,
        Email = model.Email,
        LockoutEnabled = true
      };
    }

    public async Task<IdentityResult> RegisterAsync(
      IdentityUser user,
      string password
    )
    {
      return await this.manager.CreateAsync((TIdentityUser)user, password);
    }

    public async Task<IdentityResult> ChangePasswordAsync(
      ChangePasswordViewModel model
    )
    {
      var user = await this.manager.FindByNameAsync(model.UserName);

      return await this.manager.ChangePasswordAsync(
        user,
        model.CurrentPassword,
        model.NewPassword
      );
    }

    public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
    {
      // TODO: Paging
      var items = await this.manager.Users
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
      var user = await this.manager.FindByIdAsync(id);
      var roles = await this.manager.GetRolesAsync(user);
      var claims = await this.manager.GetClaimsAsync(user);

      var isLockedOut = await this.manager.IsLockedOutAsync(user);

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

      var user = await this.manager.FindByIdAsync(model.Id);
      user.UserName = model.UserName;
      user.Email = model.Email;
      user.LockoutEnabled = model.LockoutEnabled;

      var result = await this.manager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        return result;
      }

      result = await this.AssignClaimsAsync(
        user,
        model.Claims.Select(x => new Claim(x.Type, x.Value)).ToList()
      );
      if (!result.Succeeded)
      {
        return result;
      }

      result = await this.AssignRolesAsync(user, model.Roles);
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
      var existingClaims = await this.manager.GetClaimsAsync(user);
      await this.manager.RemoveClaimsAsync(user, existingClaims);

      // assigning claims
      return await this.manager.AddClaimsAsync(
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
      var existingRoles = await this.manager.GetRolesAsync(user);
      await this.manager.RemoveFromRolesAsync(user, existingRoles);

      // assigning roles
      return await this.manager.AddToRolesAsync(
        user,
        roles
      );
    }
  }
}
