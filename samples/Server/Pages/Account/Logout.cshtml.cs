using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace tomware.Microip.Web.Pages.Account
{
  [AllowAnonymous]
  public class LogoutModel : PageModel
  {
    [BindProperty]
    public string LogoutId { get; set; }

    [BindProperty]
    public string RedirectUri { get; set; }

    private readonly ILogger<LogoutModel> logger;
    private readonly SignInManager<ApplicationUser> signInManager;

    public LogoutModel(
      ILogger<LogoutModel> logger,
      SignInManager<ApplicationUser> signInManager
    )
    {
      this.logger = logger;
      this.signInManager = signInManager;
    }

    public void OnGet(string logoutId, string redirectUri)
    {
      this.LogoutId = logoutId;
      this.RedirectUri = redirectUri;
    }

    public async Task<IActionResult> OnPost()
    {
      var vm = BuildLoggedOutViewModelAsync(LogoutId);

      if (User?.Identity.IsAuthenticated == true)
      {
        await signInManager.SignOutAsync();

        logger.LogInformation("User logged out.");
      }

      if (vm.TriggerExternalSignout)
      {
        // build a return URL so the upstream provider will redirect back
        // to us after the user has logged out. this allows us to then
        // complete our single sign-out processing.
        string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

        // this triggers a redirect to the external provider for sign-out
        return SignOut(
          new AuthenticationProperties { RedirectUri = url },
          vm.ExternalAuthenticationScheme
        );
      }

      if (this.RedirectUri != null)
      {
        return Redirect(this.RedirectUri);
      }
      else
      {
        return RedirectToPage();
      }
    }

    private LoggedOutViewModel BuildLoggedOutViewModelAsync(string logoutId)
    {
      var vm = new LoggedOutViewModel
      {
        LogoutId = logoutId
      };

      if (User?.Identity.IsAuthenticated == true)
      {
        var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
        if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
        {
          vm.ExternalAuthenticationScheme = idp;
        }
      }

      return vm;
    }
  }

  public class LoggedOutViewModel
  {
    public string PostLogoutRedirectUri { get; set; }
    public string SignOutIframeUrl { get; set; }
    public string LogoutId { get; set; }
    public string ExternalAuthenticationScheme { get; set; }
    public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
  }
}
