using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Api
{
  [ApiExplorerSettings(GroupName = ApiGroups.OPENIDDICT_UI_GROUP)]
  [Authorize(
    Policies.OPENIDDICT_UI_API_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  public class OpenIddictApiController : ApiControllerBase
  {
  }
}