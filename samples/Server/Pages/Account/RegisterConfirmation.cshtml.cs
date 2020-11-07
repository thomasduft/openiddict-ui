using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace tomware.Microip.Web.Pages.Account
{
  [AllowAnonymous]
  public class RegisterConfirmationModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IEmailSender sender;

    public RegisterConfirmationModel(
      UserManager<ApplicationUser> userManager,
      IEmailSender sender
    )
    {
      this.userManager = userManager;
      this.sender = sender;
    }

    public string Email { get; set; }

    public bool DisplayConfirmAccountLink { get; set; }

    public string EmailConfirmationUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string email)
    {
      if (email == null)
      {
        return RedirectToPage("/Index");
      }

      var user = await userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return NotFound($"Unable to load user with email '{email}'.");
      }

      Email = email;
      // Once you add a real email sender, you should remove this code that lets you confirm the account
      DisplayConfirmAccountLink = true;
      if (DisplayConfirmAccountLink)
      {
        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        EmailConfirmationUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = userId, code = code },
            protocol: Request.Scheme);
      }

      return Page();
    }
  }
}
