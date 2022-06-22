using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerWithCustomKey.Models;

namespace ServerWithCustomKey.Pages.Account.Manage;

public partial class IndexModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;

  public IndexModel(
    UserManager<ApplicationUser> userManager
  )
  {
    _userManager = userManager;
  }

  public string Username { get; set; }

  public string PhoneNumber { get; set; }


  public async Task<IActionResult> OnGetAsync()
  {
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
      return NotFound(FormattableString
        .Invariant($"Unable to load user with ID '{_userManager.GetUserId(User)}'."));
    }

    await LoadAsync(user);

    return Page();
  }

  private async Task LoadAsync(ApplicationUser user)
  {
    var userName = await _userManager.GetUserNameAsync(user);
    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

    Username = userName;
    PhoneNumber = phoneNumber;
  }
}
