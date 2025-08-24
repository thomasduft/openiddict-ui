using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using tomware.OpenIddict.UI.Identity.Core;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Api;

public interface IAccountApiService
{
  public Task<IdentityResult> RegisterAsync(RegisterUserViewModel model);
  public Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);
  public Task<IEnumerable<UserViewModel>> GetUsersAsync();
  public Task<UserViewModel> GetUserAsync(string id);
  public Task<IdentityResult> UpdateAsync(UserViewModel model);
  public Task<IdentityResult> DeleteAsync(string id);
}

public class AccountApiService<TIdentityUser, TKey> : IAccountApiService
  where TKey : IEquatable<TKey>
  where TIdentityUser : IdentityUser<TKey>, new()
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

    return items.Select(u =>
    {
      return new UserViewModel
      {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        LockoutEnabled = u.LockoutEnabled
      };
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
      Claims = [.. user.Claims.Select(x =>
      {
        return new ClaimViewModel
        {
          Type = x.Type,
          Value = x.Value
        };
      })],
      Roles = user.Roles
    };
  }

  public async Task<IdentityResult> UpdateAsync(UserViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    if (string.IsNullOrWhiteSpace(model.Id))
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var param = SimpleMapper.From<UserViewModel, UserParam>(model);
    param.Claims = [.. model.Claims.Select(c =>
    {
      return new ClaimInfo
      {
        Type = c.Type,
        Value = c.Value
      };
    })];

    return await _accountService.UpdateAsync(param);
  }

  public async Task<IdentityResult> DeleteAsync(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      throw new InvalidOperationException(nameof(id));
    }

    return await _accountService.DeleteAsync(id);
  }
}
