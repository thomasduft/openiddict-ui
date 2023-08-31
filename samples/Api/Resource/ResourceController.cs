using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Api.Resource;

[ApiController]
[Route("api/Resource")]
public class ResourceController : ControllerBase
{
  [HttpGet("private")]
  [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
  public IActionResult Private()
  {
    if (User.Identity is not ClaimsIdentity identity)
    {
      return BadRequest();
    }

    var info = new ResourceInfo
    {
      Message = $"You have authorized access to resources belonging to {identity.Name} on Api."
    };

    return Ok(info);
  }

  [HttpGet("public")]
  public IActionResult Public()
  {
    var info = new ResourceInfo
    {
      Message = "This is a public endpoint that is at Api - it does not require authorization."
    };

    return Ok(info);
  }
}
