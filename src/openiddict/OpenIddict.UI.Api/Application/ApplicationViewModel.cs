using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Api;

public class ApplicationViewModel
{
  public string Id { get; set; }

  [Required(AllowEmptyStrings = false)]
  public string ClientId { get; set; }

  public string DisplayName { get; set; }

  public string Type { get; set; } = ClientTypes.Public;

  [RequiredIfClientTypeIs(ClientTypes.Confidential)]
  public string ClientSecret { get; set; }

  public bool RequirePkce { get; set; }

  public bool RequireConsent { get; set; }

  public List<string> Permissions { get; set; } = new List<string>();
  public List<string> RedirectUris { get; set; } = new List<string>();
  public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
}
