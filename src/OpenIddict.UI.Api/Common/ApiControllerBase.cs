using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using System.Net.Mime;

namespace tomware.OpenIddict.UI.Api
{
  [Authorize(
    Policies.ADMIN_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  [Produces(MediaTypeNames.Application.Json)]
  public class ApiControllerBase : ControllerBase
  {
  }
}