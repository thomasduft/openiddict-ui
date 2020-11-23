using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace tomware.OpenIddict.UI.Api
{
  [Route("accounts")]
  public class AccountController : ApiControllerBase
  {
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountApiService _service;

    public AccountController(
      ILogger<AccountController> logger,
      IAccountApiService service
    )
    {
      _logger = logger;
      _service = service;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody]RegisterUserViewModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _service.RegisterAsync(model);
        if (result.Succeeded)
        {
          _logger.LogInformation(
            "New user {Email} successfully registred!",
            model.Email
          );

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
      if (string.IsNullOrWhiteSpace(id)) return BadRequest();

      var result = await _service.GetUserAsync(id);

      return Ok(result);
    }

    [HttpPut("user")]
    [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync([FromBody]UserViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.UpdateAsync(model);

      return Ok(result);
    }

    [HttpDelete("user/{id}")]
    [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) return BadRequest();

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
}
