using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LogoutModel : PageModel
  {
    [BindProperty]
    public string LogoutId { get; set; }

    private readonly ILogger<LogoutModel> logger;
    private readonly IEventService eventService;
    private readonly IIdentityServerInteractionService interaction;
    private readonly SignInManager<ApplicationUser> signInManager;

    public LogoutModel(
      ILogger<LogoutModel> logger,
      IEventService eventService,
      IIdentityServerInteractionService interaction,
      SignInManager<ApplicationUser> signInManager
    )
    {
      this.logger = logger;
      this.signInManager = signInManager;
      this.eventService = eventService;
      this.interaction = interaction;
    }

    public void OnGet(string logoutId)
    {
      this.LogoutId = logoutId;
    }

    public async Task<IActionResult> OnPost()
    {
      // see: https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/blob/master/Quickstart/Account/AccountController.cs
      // see: https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/blob/master/Views/Account/LoggedOut.cshtml
      // see: https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/blob/master/wwwroot/js/signout-redirect.js
      var vm = await BuildLoggedOutViewModelAsync(LogoutId);

      if (User?.Identity.IsAuthenticated == true)
      {
        await signInManager.SignOutAsync();

        await eventService.RaiseAsync(new UserLogoutSuccessEvent(
          User.GetSubjectId(),
          User.GetDisplayName()
        ));

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

      if (vm.PostLogoutRedirectUri != null)
      {
        return Redirect(vm.PostLogoutRedirectUri);
      }
      else
      {
        return RedirectToPage();
      }
    }

    private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
    {
      // get context information (client name, post logout redirect URI and iframe for federated signout)
      var logout = await this.interaction.GetLogoutContextAsync(logoutId);

      var vm = new LoggedOutViewModel
      {
        PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
        SignOutIframeUrl = logout?.SignOutIFrameUrl,
        LogoutId = logoutId
      };

      if (User?.Identity.IsAuthenticated == true)
      {
        var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
        if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
        {
          var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
          if (providerSupportsSignout)
          {
            if (vm.LogoutId == null)
            {
              // if there's no current logout context, we need to create one
              // this captures necessary info from the current logged in user
              // before we signout and redirect away to the external IdP for signout
              vm.LogoutId = await this.interaction.CreateLogoutContextAsync();
            }

            vm.ExternalAuthenticationScheme = idp;
          }
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
