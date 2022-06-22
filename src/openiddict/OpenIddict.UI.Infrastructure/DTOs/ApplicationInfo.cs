using System.Collections.Generic;

namespace tomware.OpenIddict.UI.Infrastructure;

public class ApplicationInfo
{
  public string Id { get; set; }

  public string ClientId { get; set; }

  public string DisplayName { get; set; }

  public string ClientSecret { get; set; }

  public bool RequirePkce { get; set; }

  public bool RequireConsent { get; set; }

  public List<string> Permissions { get; set; } = new List<string>();
  public List<string> RedirectUris { get; set; } = new List<string>();
  public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();

  public string Type { get; set; }
}
