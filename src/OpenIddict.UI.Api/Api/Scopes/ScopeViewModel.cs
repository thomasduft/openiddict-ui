using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class ScopeViewModel
  {
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }
  }
}
