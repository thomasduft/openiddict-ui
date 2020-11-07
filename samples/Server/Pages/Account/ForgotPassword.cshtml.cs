using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account
{
  [AllowAnonymous]
  public class ForgotPasswordModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEmailSender emailSender;
    private readonly IdentityLocalizationService identityLocalizationService;

    public ForgotPasswordModel(
      UserManager<ApplicationUser> userManager,
      IEmailSender emailSender,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.emailSender = emailSender;
      this.identityLocalizationService = identityLocalizationService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
      [Required]
      [EmailAddress]
      public string Email { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
        {
          // Don't reveal that the user does not exist or is not confirmed
          return RedirectToPage("./ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code },
            protocol: Request.Scheme);

        await emailSender.SendEmailAsync(
            Input.Email,
            this.identityLocalizationService.GetLocalizedHtmlString("RESET_PASSWORD"),
            this.identityLocalizationService.GetLocalizedHtmlString("RESET_PASSWORD_EMAIL_TEXT", HtmlEncoder.Default.Encode(callbackUrl))
          );

        return RedirectToPage("./ForgotPasswordConfirmation");
      }

      return Page();
    }
  }
}
