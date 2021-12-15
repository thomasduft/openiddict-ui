using System;
using Microsoft.AspNetCore.Authorization;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Api
{
  public class OpenIddictUIIdentityApiOptions
  {
    /// <summary>
    /// Registers a conventional route prefix for the API controllers. Defaults to "api/".
    /// </summary>
    public string RoutePrefix { get; set; } = "api/";

    /// <summary>
    /// Allows to register custom authorization policies for accessing OpenIddict-UI Identity API's.
    /// <example>Defaults to:
    /// <code>
    /// policy
    ///   .RequireAuthenticatedUser()
    ///   .RequireRole(Roles.ADMINISTRATOR_ROLE);
    /// </code>
    /// </example>
    /// </summary>
    public Action<AuthorizationPolicyBuilder> Policy { get; set; } = policy =>
      policy
        .RequireAuthenticatedUser()
        .RequireRole(Roles.ADMINISTRATOR_ROLE);
  }
}