using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public class GenerateRecoveryCodesModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<GenerateRecoveryCodesModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public GenerateRecoveryCodesModel(
      UserManager<ApplicationUser> userManager,
      ILogger<GenerateRecoveryCodesModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    [TempData]
    public string[] RecoveryCodes { get; set; }

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

      var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
      if (!isTwoFactorEnabled)
      {
        var userId = await userManager.GetUserIdAsync(user);
        throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
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

      var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
      var userId = await userManager.GetUserIdAsync(user);
      if (!isTwoFactorEnabled)
      {
        throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
      }

      var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
      RecoveryCodes = recoveryCodes.ToArray();

      logger.LogInformation(
        "User with ID '{UserId}' has generated new 2FA recovery codes.",
        userId
      );

      StatusMessage = this.identityLocalizationService
                        .GetLocalizedHtmlString("STATUS_GENERATE_CODES");

      return RedirectToPage("./ShowRecoveryCodes");
    }
  }
}