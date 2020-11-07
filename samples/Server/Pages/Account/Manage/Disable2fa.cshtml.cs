using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public class Disable2faModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<Disable2faModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public Disable2faModel(
      UserManager<ApplicationUser> userManager,
      ILogger<Disable2faModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
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

      if (!await userManager.GetTwoFactorEnabledAsync(user))
      {
        throw new InvalidOperationException($"Cannot disable 2FA for user with ID '{userManager.GetUserId(User)}' as it's not currently enabled.");
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

      var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
      if (!disable2faResult.Succeeded)
      {
        throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{userManager.GetUserId(User)}'.");
      }

      logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userManager.GetUserId(User));

      StatusMessage = this.identityLocalizationService.GetLocalizedHtmlString("DISABLE_2FA_STATUS");

      return RedirectToPage("./TwoFactorAuthentication");
    }
  }
}