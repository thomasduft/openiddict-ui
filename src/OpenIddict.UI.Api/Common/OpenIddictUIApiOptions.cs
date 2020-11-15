using System;
using System.Collections.Generic;

namespace tomware.OpenIddict.UI.Api
{
  public class OpenIddictUIApiOptions
  {
    public HashSet<string> Permissions { get; set; } = new HashSet<string>(StringComparer.Ordinal);
    public HashSet<string> Requirements { get; set; } = new HashSet<string>(StringComparer.Ordinal);
  }
}