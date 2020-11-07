using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public class ExternalLoginsModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IdentityLocalizationService identityLocalizationService;

    public ExternalLoginsModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.identityLocalizationService = identityLocalizationService;
    }

    public IList<UserLoginInfo> CurrentLogins { get; set; }

    public IList<AuthenticationScheme> OtherLogins { get; set; }

    public bool ShowRemoveButton { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      CurrentLogins = await userManager.GetLoginsAsync(user);
      OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
          .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
          .ToList();

      ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;

      return Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(
      string loginProvider,
      string providerKey
    )
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
      if (!result.Succeeded)
      {
        StatusMessage = this.identityLocalizationService
          .GetLocalizedHtmlString("STATUS_EXTERNAL_LOGIN_REMOVED");

        return RedirectToPage();
      }

      await signInManager.RefreshSignInAsync(user);

      StatusMessage = this.identityLocalizationService
          .GetLocalizedHtmlString("STATUS_EXTERNAL_LOGIN_REMOVED");

      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
      // Clear the existing external cookie to ensure a clean login process
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

      // Request a redirect to the external login provider to link a login for the current user
      var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
      var properties = signInManager.ConfigureExternalAuthenticationProperties(
                         provider,
                         redirectUrl,
                         userManager.GetUserId(User)
                       );

      return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
          .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      var info = await signInManager
        .GetExternalLoginInfoAsync(await userManager.GetUserIdAsync(user));
      if (info == null)
      {
        throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
      }

      var result = await userManager.AddLoginAsync(user, info);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
      }

      // Clear the existing external cookie to ensure a clean login process
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

      StatusMessage = this.identityLocalizationService
                        .GetLocalizedHtmlString("STATUS_EXTERNAL_LOGIN_ADDED");

      return RedirectToPage();
    }
  }
}
