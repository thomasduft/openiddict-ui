using System.Collections.Generic;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Api;

public class ApplicationOptionsViewModel
{
  public List<string> Permissions { get; set; } = [];

  public List<string> Requirements { get; set; } = [];

  public List<string> Types { get; set; } = [
    ClientTypes.Public,
    ClientTypes.Confidential
  ];
}
