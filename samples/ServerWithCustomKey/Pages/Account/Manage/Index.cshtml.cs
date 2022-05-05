using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerWithCustomKey.Models;

namespace ServerWithCustomKey.Pages.Account.Manage
{
  public partial class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;

    public IndexModel(
      UserManager<ApplicationUser> userManager
    )
    {
      this.userManager = userManager;
    }

    public string Username { get; set; }

    public string PhoneNumber { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(
          string.Format("Unable to load user with ID '{0}'.",
          userManager.GetUserId(User))
        );
      }

      await LoadAsync(user);

      return Page();
    }

    private async Task LoadAsync(ApplicationUser user)
    {
      var userName = await userManager.GetUserNameAsync(user);
      var phoneNumber = await userManager.GetPhoneNumberAsync(user);

      Username = userName;
      PhoneNumber = phoneNumber;
    }
  }
}
