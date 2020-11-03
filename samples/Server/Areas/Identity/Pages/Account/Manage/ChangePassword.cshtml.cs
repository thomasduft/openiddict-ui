using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account.Manage
{
  public class ChangePasswordModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<ChangePasswordModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public ChangePasswordModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<ChangePasswordModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class InputModel
    {
      [Required(ErrorMessage = "CURRENT_PASSWORD_REQUIRED")]
      [DataType(DataType.Password)]
      public string OldPassword { get; set; }

      [Required(ErrorMessage = "NEW_PASSWORD_REQUIRED")]
      [StringLength(
        100,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6
      )]
      [DataType(DataType.Password)]
      public string NewPassword { get; set; }

      [DataType(DataType.Password)]
      [Compare(
        "NewPassword",
        ErrorMessage = "CONFIRM_PASSWORD_NOT_MATCHING"
      )]
      public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
      }

      var hasPassword = await userManager.HasPasswordAsync(user);
      if (!hasPassword)
      {
        return RedirectToPage("./SetPassword");
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
        return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
      }

      var changePasswordResult = await userManager.ChangePasswordAsync(
          user,
          Input.OldPassword,
          Input.NewPassword
      );

      if (!changePasswordResult.Succeeded)
      {
        foreach (var error in changePasswordResult.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
      }

      await signInManager.RefreshSignInAsync(user);

      logger.LogInformation("User changed their password successfully.");

      StatusMessage = this.identityLocalizationService
        .GetLocalizedHtmlString("CHANGE_PASSWORD_STATUS");

      return RedirectToPage();
    }
  }
}
