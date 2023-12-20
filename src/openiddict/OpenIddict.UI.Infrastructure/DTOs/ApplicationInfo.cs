using System.Collections.Generic;

namespace tomware.OpenIddict.UI.Infrastructure;

public class ApplicationInfo
{
  public string Id { get; set; }

  public string ClientId { get; set; }

  public string DisplayName { get; set; }

  public bool RequirePkce { get; set; }

  public bool RequireConsent { get; set; }

  public List<string> Permissions { get; set; } = [];
  public List<string> RedirectUris { get; set; } = [];
  public List<string> PostLogoutRedirectUris { get; set; } = [];

  public string Type { get; set; }
}
