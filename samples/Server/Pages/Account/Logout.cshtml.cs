using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Models;

namespace Server.Pages.Account;

[AllowAnonymous]
public class LogoutModel : PageModel
{
  private readonly ILogger<LogoutModel> _logger;
  private readonly SignInManager<ApplicationUser> _signInManager;

  [BindProperty]
  public string LogoutId { get; set; }

  [BindProperty]
  public string RedirectUri { get; set; }

  public LogoutModel(
    ILogger<LogoutModel> logger,
    SignInManager<ApplicationUser> signInManager
  )
  {
    _logger = logger;
    _signInManager = signInManager;
  }

  public void OnGet(string logoutId, string redirectUri)
  {
    LogoutId = logoutId;
    RedirectUri = redirectUri;
  }

  public async Task<IActionResult> OnPost()
  {
    if (User?.Identity.IsAuthenticated == true)
    {
      await _signInManager.SignOutAsync();

      _logger.LogInformation("User logged out.");
    }

    if (RedirectUri != null)
    {
      return Redirect(RedirectUri);
    }
    else
    {
      return RedirectToPage();
    }
  }
}
