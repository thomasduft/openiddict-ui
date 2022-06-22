using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI.Identity.Api;

[Route("accounts")]
public class AccountController : IdentityApiController
{
  private readonly IAccountApiService _service;

  public AccountController(
    IAccountApiService service
  )
  {
    _service = service;
  }

  [HttpPost("register")]
  [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
  public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
  {
    if (model == null)
    {
      return BadRequest();
    }

    if (ModelState.IsValid)
    {
      var result = await _service.RegisterAsync(model);
      if (result.Succeeded)
      {
        return Ok(result);
      }

      AddErrors(result);
    }

    return BadRequest(ModelState);
  }

  [HttpPost("changepassword")]
  [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
  {
    if (model == null)
    {
      return BadRequest();
    }

    if (ModelState.IsValid)
    {
      var result = await _service.ChangePasswordAsync(model);
      if (result.Succeeded)
      {
        return Ok(result);
      }

      AddErrors(result);
    }

    return BadRequest(ModelState);
  }

  [HttpGet("users")]
  [ProducesResponseType(typeof(IEnumerable<UserViewModel>), StatusCodes.Status200OK)]
  public async Task<IActionResult> Users()
  {
    var result = await _service.GetUsersAsync();

    return Ok(result);
  }

  [HttpGet("user/{id}")]
  [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetUser(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return BadRequest();
    }

    var result = await _service.GetUserAsync(id);

    return Ok(result);
  }

  [HttpPut("user")]
  [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
  public async Task<IActionResult> UpdateAsync([FromBody] UserViewModel model)
  {
    if (model == null)
    {
      return BadRequest();
    }

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var result = await _service.UpdateAsync(model);

    return Ok(result);
  }

  [HttpDelete("user/{id}")]
  [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
  public async Task<IActionResult> DeleteAsync(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return BadRequest();
    }

    // TODO: Don't think deleting user makes sense
    // Better provide an Inactive property on the IdentityUser and set it to Inactive.
    // throw new NotImplementedException("DeleteAsync");

    // Proposal: UserDeletionStrategy???

    var result = await _service.DeleteAsync(id);

    return Ok(result);
  }

  private void AddErrors(IdentityResult result)
  {
    foreach (var error in result.Errors)
    {
      ModelState.AddModelError(string.Empty, error.Description);
    }
  }
}
