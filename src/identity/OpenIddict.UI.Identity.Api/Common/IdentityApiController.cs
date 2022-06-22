using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Identity.Api;

[ApiExplorerSettings(GroupName = ApiGroups.OPENIDDICTUIIDENTITYGROUP)]
[Authorize(
  Policies.OPENIDDICTUIIDENTITYAPIPOLICY,
  AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
)]
public class IdentityApiController : ApiControllerBase
{
}
