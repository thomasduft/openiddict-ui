using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public class ResetAuthenticatorModel : PageModel
  {
    UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    ILogger<ResetAuthenticatorModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public ResetAuthenticatorModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<ResetAuthenticatorModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      await userManager.SetTwoFactorEnabledAsync(user, false);
      await userManager.ResetAuthenticatorKeyAsync(user);
      logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

      await signInManager.RefreshSignInAsync(user);

      StatusMessage = this.identityLocalizationService
                        .GetLocalizedHtmlString("RESET_AUTHENTICATOR_STATUS");

      return RedirectToPage("./EnableAuthenticator");
    }
  }
}