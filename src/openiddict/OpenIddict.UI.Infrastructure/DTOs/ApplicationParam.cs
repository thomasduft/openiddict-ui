using System.Collections.Generic;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Infrastructure;

public class ApplicationParam
{
  public string Id { get; set; }
  public string ClientId { get; set; }
  public string DisplayName { get; set; }
  public string ClientSecret { get; set; }

  /// <summary>
  /// Impacts the requirements.
  /// </summary>
  public bool RequirePkce { get; set; }

  /// <summary>
  /// Impacts the ConsentType. True is explicit, False is implicit.
  /// </summary>
  public bool RequireConsent { get; set; }

  public List<string> Permissions { get; set; } = new List<string>();
  public List<string> RedirectUris { get; set; } = new List<string>();
  public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
  public List<string> Requirements { get; set; } = new List<string>();
  public string Type { get; set; } = ClientTypes.Public;
}
