using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using tomware.Microip.Web.Resources;

namespace tomware.Microip.Web.Areas.Identity.Pages.Account.Manage
{
  public class PersonalDataModel : PageModel
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<PersonalDataModel> logger;
    private readonly IdentityLocalizationService identityLocalizationService;

    public PersonalDataModel(
      UserManager<ApplicationUser> userManager,
      ILogger<PersonalDataModel> logger,
      IdentityLocalizationService identityLocalizationService
    )
    {
      this.userManager = userManager;
      this.logger = logger;
      this.identityLocalizationService = identityLocalizationService;
    }

    public async Task<IActionResult> OnGet()
    {
      var user = await userManager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound(this.identityLocalizationService
         .GetLocalizedHtmlString("USER_NOTFOUND", userManager.GetUserId(User)));
      }

      return Page();
    }
  }
}