using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Core;

public interface IAccountService
{
  public Task<IdentityResult> RegisterAsync(RegisterUserParam model);
  public Task<IdentityResult> ChangePasswordAsync(ChangePasswordParam model);
  public Task<IEnumerable<UserInfo>> GetUsersAsync();
  public Task<UserInfo> GetUserAsync(string id);
  public Task<IdentityResult> UpdateAsync(UserParam model);
  public Task<IdentityResult> DeleteAsync(string id);
}
