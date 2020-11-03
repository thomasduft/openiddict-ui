using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class ConfirmEmailChangeModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IdentityLocalizationService identityLocalizationService;

    public ConfirmEmailChangeModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.identityLocalizationService = identityLocalizationService;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
    {
      if (userId == null || email == null || code == null)
      {
        return RedirectToPage("/Index");
      }

      var user = await userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID '{userId}'.");
      }

      code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
      var result = await userManager.ChangeEmailAsync(user, email, code);
      if (!result.Succeeded)
      {
        StatusMessage = identityLocalizationService.GetLocalizedHtmlString("CONFIRM_EMAIL_CHANGE_ERROR");
        return Page();
      }

      // // In our UI email and user name are one and the same, so when we update the email
      // // we need to update the user name.
      // var setUserNameResult = await userManager.SetUserNameAsync(user, email);
      // if (!setUserNameResult.Succeeded)
      // {
      //   StatusMessage = "Error changing user name.";
      //   return Page();
      // }

      await signInManager.RefreshSignInAsync(user);

      StatusMessage = identityLocalizationService.GetLocalizedHtmlString("CONFIRM_EMAIL_CHANGE_TEXT");

      return Page();
    }
  }
}
