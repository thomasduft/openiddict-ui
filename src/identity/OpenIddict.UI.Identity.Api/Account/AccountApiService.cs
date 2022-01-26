using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using tomware.OpenIddict.UI.Identity.Core;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public interface IAccountApiService
  {
    Task<IdentityResult> RegisterAsync(RegisterUserViewModel model);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
    Task<UserViewModel> GetUserAsync(string id);
    Task<IdentityResult> UpdateAsync(UserViewModel model);
    Task<IdentityResult> DeleteAsync(string id);
  }

  public class AccountApiService<TIdentityUser> : IAccountApiService
    where TIdentityUser : IdentityUser, new()
  {
    private readonly IAccountService _accountService;

    public AccountApiService(
      IAccountService accountService
    )
    {
      _accountService = accountService
        ?? throw new ArgumentNullException(nameof(accountService));
    }

    public async Task<IdentityResult> RegisterAsync(
      RegisterUserViewModel model
    )
    {
      var param = SimpleMapper.From<RegisterUserViewModel, RegisterUserParam>(model);

      return await _accountService.RegisterAsync(param);
    }

    public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
    {
      var param = SimpleMapper.From<ChangePasswordViewModel, ChangePasswordParam>(model);

      return await _accountService.ChangePasswordAsync(param);
    }

    public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
    {
      // TODO: Paging ???
      var items = await _accountService.GetUsersAsync();

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
      var user = await _accountService.GetUserAsync(id);

      return new UserViewModel
      {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        LockoutEnabled = user.LockoutEnabled,
        IsLockedOut = user.IsLockedOut,
        Claims = new List<ClaimViewModel>(user.Claims.Select(x => new ClaimViewModel
        {
          Type = x.Type,
          Value = x.Value
        })),
        Roles = user.Roles
      };
    }

    public async Task<IdentityResult> UpdateAsync(UserViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));
      if (string.IsNullOrWhiteSpace(model.Id))
        throw new InvalidOperationException(nameof(model.Id));

      var param = SimpleMapper.From<UserViewModel, UserParam>(model);
      param.Claims = new List<ClaimInfo>(model.Claims.Select(c => new ClaimInfo
      {
        Type = c.Type,
        Value = c.Value
      }));

      return await _accountService.UpdateAsync(param);
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException(nameof(id));

      return await _accountService.DeleteAsync(id);
    }
  }
}
