using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using tomware.OpenIddict.UI.Suite.Api;

namespace tomware.OpenIddict.UI.Identity.Api
{
  [ApiExplorerSettings(GroupName = ApiGroups.OPENIDDICT_UI_IDENTITY_GROUP)]
  [Authorize(
    Policies.OPENIDDICT_UI_IDENTITY_API_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  public class IdentityApiController : ApiControllerBase
  {
  }
}