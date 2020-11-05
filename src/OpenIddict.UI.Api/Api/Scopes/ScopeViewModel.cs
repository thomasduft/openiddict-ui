using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class ScopeViewModel
  {
    public int? Id { get; set; }

    public bool Enabled { get; set; }

    [Required]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool Required { get; set; } = false;

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; }

    public List<string> UserClaims { get; set; } = new List<string>();
  }
}
