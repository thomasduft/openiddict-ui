using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public class RoleViewModel
  {
    public string Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }
  }
}
