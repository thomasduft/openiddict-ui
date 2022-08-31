using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Api;

[ApiExplorerSettings(GroupName = ApiGroups.OpenIddictUiApi)]
[Authorize(
  Policies.OpenIddictUiApiPolicy,
  AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
)]
public class OpenIddictApiController : ApiControllerBase
{
}
