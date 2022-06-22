using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public class ClaimViewModel
  {
    public string Type { get; set; }
    public string Value { get; set; }
  }

  public class UserViewModel
  {
    [Required(AllowEmptyStrings = false)]
    public string Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string UserName { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Email { get; set; }

    public bool LockoutEnabled { get; set; }

    public bool IsLockedOut { get; set; }

    public List<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();

    public List<string> Roles { get; set; } = new List<string>();
  }
}