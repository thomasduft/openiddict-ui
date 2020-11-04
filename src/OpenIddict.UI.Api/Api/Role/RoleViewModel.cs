using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class RoleViewModel
  {
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }
  }
}
