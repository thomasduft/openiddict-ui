using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LoginWithRecoveryCodeModel : PageModel
  {
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<LoginWithRecoveryCodeModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public LoginWithRecoveryCodeModel(
      SignInManager<ApplicationUser> signInManager,
      ILogger<LoginWithRecoveryCodeModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.signInManager = signInManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
      [BindProperty]
      [Required]
      [DataType(DataType.Text)]
      [Display(Name = "Recovery Code")]
      public string RecoveryCode { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
      // Ensure the user has gone through the username & password screen first
      var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
      if (user == null)
      {
        throw new InvalidOperationException($"Unable to load two-factor authentication user.");
      }

      ReturnUrl = returnUrl;

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
      if (user == null)
      {
        throw new InvalidOperationException($"Unable to load two-factor authentication user.");
      }

      var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

      var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

      if (result.Succeeded)
      {
        logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
      }
      if (result.IsLockedOut)
      {
        logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
        return RedirectToPage("./Lockout");
      }
      else
      {
        logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);

        ModelState.AddModelError(
          string.Empty,
          this.identityLocalizationService.GetLocalizedHtmlString("INVALID_RECOVERY_CODE")
        );

        return Page();
      }
    }
  }
}
