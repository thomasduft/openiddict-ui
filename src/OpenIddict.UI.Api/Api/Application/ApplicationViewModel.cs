using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class ApplicationViewModel
  {
    public string Id { get; set; }

    [Required]
    public string ClientId { get; set; }

    public string DisplayName { get; set; }

    public string ClientSecret { get; set; }

    public bool RequirePkce { get; set; }

    public bool RequireConsent { get; set; }

    public List<string> RedirectUris { get; set; } = new List<string>();
    public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
    public List<string> Permissions { get; set; } = new List<string>();
    public List<string> Properties { get; set; } = new List<string>();

    public string Type { get; set; }
  }
}
