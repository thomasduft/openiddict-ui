using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServerWithCustomKey.Pages.Account;

[AllowAnonymous]
public class LockoutModel : PageModel
{
  public void OnGet()
  {

  }
}
