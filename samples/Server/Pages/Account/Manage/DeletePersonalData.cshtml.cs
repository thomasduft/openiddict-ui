using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public class DeletePersonalDataModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<DeletePersonalDataModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public DeletePersonalDataModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<DeletePersonalDataModel> logger,
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

    public class InputModel
    {
      [Required]
      [DataType(DataType.Password)]
      public string Password { get; set; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
          .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      RequirePassword = await userManager.HasPasswordAsync(user);

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

      RequirePassword = await userManager.HasPasswordAsync(user);
      if (RequirePassword)
      {
        if (!await userManager.CheckPasswordAsync(user, Input.Password))
        {
          ModelState.AddModelError(
            string.Empty,
            this.identityLocalizationService.GetLocalizedHtmlString("INCORRECT_PASSWORD")
          );

          return Page();
        }
      }

      var result = await userManager.DeleteAsync(user);
      var userId = await userManager.GetUserIdAsync(user);
      if (!result.Succeeded)
      {
        throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
      }

      await signInManager.SignOutAsync();

      logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

      return Redirect("~/");
    }
  }
}
