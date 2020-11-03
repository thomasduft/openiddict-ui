using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account.Manage
{
  public partial class EmailModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IEmailSender emailSender;
    private readonly IdentityLocalizationService identityLocalizationService;

    public EmailModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IEmailSender emailSender,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.emailSender = emailSender;
      this.identityLocalizationService = identityLocalizationService;
    }

    public string Username { get; set; }

    public string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
      [Required(ErrorMessage = "EMAIL_REQUIRED")]
      [EmailAddress]
      public string NewEmail { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
      var email = await userManager.GetEmailAsync(user);
      Email = email;

      Input = new InputModel
      {
        NewEmail = email,
      };

      IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      await LoadAsync(user);

      return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      if (!ModelState.IsValid)
      {
        await LoadAsync(user);

        return Page();
      }

      var email = await userManager.GetEmailAsync(user);
      if (Input.NewEmail != email)
      {
        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmailChange",
            pageHandler: null,
            values: new { userId = userId, email = Input.NewEmail, code = code },
            protocol: Request.Scheme);
        await emailSender.SendEmailAsync(
            Input.NewEmail,
            this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL"),
            this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_TEXT", HtmlEncoder.Default.Encode(callbackUrl))
          );

        StatusMessage = this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_STATUS_TEXT");

        return RedirectToPage();
      }

      StatusMessage = this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_UNCHANGED_TEXT");

      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
          .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      if (!ModelState.IsValid)
      {
        await LoadAsync(user);
        return Page();
      }

      var userId = await userManager.GetUserIdAsync(user);
      var email = await userManager.GetEmailAsync(user);
      var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
      var callbackUrl = Url.Page(
          "/Account/ConfirmEmail",
          pageHandler: null,
          values: new { area = "Identity", userId = userId, code = code },
          protocol: Request.Scheme);
      await emailSender.SendEmailAsync(
          email,
            this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL"),
            this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_TEXT", HtmlEncoder.Default.Encode(callbackUrl))
          );

      StatusMessage = this.identityLocalizationService.GetLocalizedHtmlString("CONFIRM_YOUR_EMAIL_STATUS_TEXT");

      return RedirectToPage();
    }
  }
}
