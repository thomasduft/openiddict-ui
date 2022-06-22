using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerWithCustomKey.Models;

namespace ServerWithCustomKey.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
  private readonly ILogger<LoginModel> _logger;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public LoginModel(
    ILogger<LoginModel> logger,
    SignInManager<ApplicationUser> signInManager
  )
  {
    _logger = logger;
    _signInManager = signInManager;
  }

  [BindProperty]
  public InputModel Input { get; set; }

  public IList<AuthenticationScheme> ExternalLogins { get; set; }

  public string ReturnUrl { get; set; }

  [TempData]
  public string ErrorMessage { get; set; }

  public class InputModel
  {
    [Required(ErrorMessage = "Username required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
  }

  public async Task OnGetAsync(string returnUrl = null)
  {
    if (!string.IsNullOrEmpty(ErrorMessage))
    {
      ModelState.AddModelError(string.Empty, ErrorMessage);
    }

    returnUrl ??= Url.Content("~/");

    // Clear the existing external cookie to ensure a clean login process
    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    ReturnUrl = returnUrl;
  }

  public async Task<IActionResult> OnPostAsync(string returnUrl = null)
  {
    returnUrl ??= Url.Content("~/");

    if (ModelState.IsValid)
    {
      var result = await _signInManager.PasswordSignInAsync(
        Input.Username,
        Input.Password,
        Input.RememberMe,
        lockoutOnFailure: true
      );
      if (result.Succeeded)
      {
        _logger.LogInformation("User logged in.");
        return LocalRedirect(returnUrl);
      }
      if (result.RequiresTwoFactor)
      {
        return RedirectToPage("./LoginWith2fa", new
        {
          ReturnUrl = returnUrl,
          Input.RememberMe
        });
      }
      if (result.IsLockedOut)
      {
        _logger.LogWarning("User account locked out.");
        return RedirectToPage("./Lockout");
      }
      else
      {
        ModelState.AddModelError(
          string.Empty,
          "Invalid login attempt"
        );

        return Page();
      }
    }

    // If we got this far, something failed, redisplay form
    return Page();
  }
}
