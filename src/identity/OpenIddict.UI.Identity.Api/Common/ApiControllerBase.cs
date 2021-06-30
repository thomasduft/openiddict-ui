using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using System.Net.Mime;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Identity.Api
{
  [Authorize(
    Policies.ADMIN_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  [Produces(MediaTypeNames.Application.Json)]
  [ApiExplorerSettings(GroupName = "openiddict-ui-identity")]
  public class ApiControllerBase : ControllerBase
  {
  }
}