using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account
{
  [AllowAnonymous]
  public class ExternalLoginModel : PageModel
  {
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEmailSender emailSender;
    private readonly IdentityLocalizationService identityLocalizationService;
    private readonly ILogger<ExternalLoginModel> logger;

    public ExternalLoginModel(
      SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager,
      ILogger<ExternalLoginModel> logger,
      IEmailSender emailSender,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.signInManager = signInManager;
      this.userManager = userManager;
      this.logger = logger;
      this.emailSender = emailSender;
      this.identityLocalizationService = identityLocalizationService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string LoginProvider { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
      [Required]
      [EmailAddress]
      public string Email { get; set; }
    }

    public IActionResult OnGetAsync()
    {
      return RedirectToPage("./Login");
    }

    public IActionResult OnPost(string provider, string returnUrl = null)
    {
      // Request a redirect to the external login provider.
      var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
      var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
    {
      returnUrl = returnUrl ?? Url.Content("~/");
      if (remoteError != null)
      {
        ErrorMessage = this.identityLocalizationService.GetLocalizedHtmlString("EXTERNAL_PROVIDER_ERROR", remoteError);
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
      }
      var info = await signInManager.GetExternalLoginInfoAsync();
      if (info == null)
      {
        ErrorMessage = this.identityLocalizationService.GetLocalizedHtmlString("EXTERNAL_PROVIDER_ERROR_INFO");
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
      }

      // Sign in the user with this external login provider if the user already has a login.
      var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
      if (result.Succeeded)
      {
        logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
        return LocalRedirect(returnUrl);
      }
      if (result.IsLockedOut)
      {
        return RedirectToPage("./Lockout");
      }
      else
      {
        // If the user does not have an account, then ask the user to create an account.
        ReturnUrl = returnUrl;
        LoginProvider = info.LoginProvider;
        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
          Input = new InputModel
          {
            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
          };
        }
        return Page();
      }
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
    {
      returnUrl = returnUrl ?? Url.Content("~/");
      // Get the information about the user from the external login provider
      var info = await signInManager.GetExternalLoginInfoAsync();
      if (info == null)
      {
        ErrorMessage = this.identityLocalizationService.GetLocalizedHtmlString("EXTERNAL_PROVIDER_ERROR_CONFIRMATION");
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
      }

      if (ModelState.IsValid)
      {
        var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
        var result = await userManager.CreateAsync(user);
        if (result.Succeeded)
        {
          result = await userManager.AddLoginAsync(user, info);
          if (result.Succeeded)
          {
            logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

            // If account confirmation is required, we need to show the link if we don't have a real email sender
            if (userManager.Options.SignIn.RequireConfirmedAccount)
            {
              return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            await emailSender.SendEmailAsync(
              Input.Email,
              this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL"),
              this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_TEXT", HtmlEncoder.Default.Encode(callbackUrl))
            );

            return LocalRedirect(returnUrl);
          }
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      LoginProvider = info.LoginProvider;
      ReturnUrl = returnUrl;

      return Page();
    }
  }
}
