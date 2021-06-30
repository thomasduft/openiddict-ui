using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using System.Net.Mime;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Api
{
  [Authorize(
    Policies.ADMIN_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  [Produces(MediaTypeNames.Application.Json)]
  [ApiExplorerSettings(GroupName = "openiddict-ui-api")]
  public class ApiControllerBase : ControllerBase
  {
  }
}