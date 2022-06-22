using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Api;

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

  /// <summary>
  /// Allows to register custom authorization policies for accessing OpenIddict-UI API's.
  /// <example>Defaults to:
  /// <code>
  /// policy
  ///   .RequireAuthenticatedUser()
  ///   .RequireRole(Roles.ADMINISTRATOR_ROLE);
  /// </code>
  /// </example>
  /// </summary>
  public Action<AuthorizationPolicyBuilder> Policy { get; set; } = policy =>
  {
    policy
      .RequireAuthenticatedUser()
      .RequireRole(Roles.Administrator);
  };
}
