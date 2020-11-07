using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Pages.Account.Manage
{
  public partial class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IdentityLocalizationService identityLocalizationService;

    public IndexModel(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.identityLocalizationService = identityLocalizationService;
    }

    public string Username { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
      [Phone]
      [Display(Name = "Phone number")]
      public string PhoneNumber { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
      var userName = await userManager.GetUserNameAsync(user);
      var phoneNumber = await userManager.GetPhoneNumberAsync(user);

      Username = userName;

      Input = new InputModel
      {
        PhoneNumber = phoneNumber
      };
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

    public async Task<IActionResult> OnPostAsync()
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

      var phoneNumber = await userManager.GetPhoneNumberAsync(user);
      if (Input.PhoneNumber != phoneNumber)
      {
        var setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        if (!setPhoneResult.Succeeded)
        {
          var userId = await userManager.GetUserIdAsync(user);
          throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
        }
      }

      await signInManager.RefreshSignInAsync(user);

      StatusMessage = this.identityLocalizationService
                        .GetLocalizedHtmlString("STATUS_UPDATE_PROFILE_EMAIL_SEND");

      return RedirectToPage();
    }
  }
}
