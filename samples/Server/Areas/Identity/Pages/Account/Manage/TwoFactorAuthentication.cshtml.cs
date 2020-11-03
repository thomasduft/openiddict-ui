using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account.Manage
{
  public class TwoFactorAuthenticationModel : PageModel
  {
    private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<TwoFactorAuthenticationModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public TwoFactorAuthenticationModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<TwoFactorAuthenticationModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

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

      HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
      Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
      IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
      RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      await signInManager.ForgetTwoFactorClientAsync();

      StatusMessage = this.identityLocalizationService
        .GetLocalizedHtmlString("2FA_STATUS_BROWSER_FORGET");

      return RedirectToPage();
    }
  }
}