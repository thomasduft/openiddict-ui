using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace tomware.Microip.Web.Pages.Account
{
  [AllowAnonymous]
  public class LockoutModel : PageModel
  {
    public void OnGet()
    {

    }
  }
}
