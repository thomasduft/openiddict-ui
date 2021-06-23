using System.Collections.Generic;

namespace tomware.OpenIddict.UI.Api
{
  public class OpenIddictUIApiOptions
  {
    /// <summary>
    /// Tell the system about the allowed Permissions it is built/configured for.
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();

    /// <summary>
    /// Registers a conventional route prefix for the API controllers. Defaults to "api/".
    /// </summary>
    public string RoutePrefix { get; set; } = "api/";
  }
}