using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account.Manage
{
  public class SetPasswordModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IdentityLocalizationService identityLocalizationService;

    public SetPasswordModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.identityLocalizationService = identityLocalizationService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class InputModel
    {
      [Required(ErrorMessage = "PASSWORD_REQUIRED")]
      [StringLength(
        100,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6
      )]
      [DataType(DataType.Password)]
      public string NewPassword { get; set; }

      [DataType(DataType.Password)]
      [Compare("NewPassword", ErrorMessage = "CONFIRM_PASSWORD_NOT_MATCHING")]
      public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      var hasPassword = await userManager.HasPasswordAsync(user);

      if (hasPassword)
      {
        return RedirectToPage("./ChangePassword");
      }

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      var addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);
      if (!addPasswordResult.Succeeded)
      {
        foreach (var error in addPasswordResult.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
      }

      await signInManager.RefreshSignInAsync(user);

      StatusMessage = this.identityLocalizationService
                        .GetLocalizedHtmlString("SET_PASSWORD_STATUS");

      return RedirectToPage();
    }
  }
}
