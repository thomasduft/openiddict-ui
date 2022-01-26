using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public interface IAccountService
  {
    Task<IdentityResult> RegisterAsync(RegisterUserParam model);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordParam model);
    Task<IEnumerable<UserInfo>> GetUsersAsync();
    Task<UserInfo> GetUserAsync(string id);
    Task<IdentityResult> UpdateAsync(UserParam model);
    Task<IdentityResult> DeleteAsync(string id);
  }
}