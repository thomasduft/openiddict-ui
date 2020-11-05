using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Api
{
  [Route("api/accounts")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class AccountController : ControllerBase
  {
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _service;

    public AccountController(
      ILogger<AccountController> logger,
      IAccountService accountService
    )
    {
      _logger = logger;
      _service = accountService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody]RegisterUserViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = _service.CreateUser(model);
        var result = await _service.RegisterAsync(user, model.Password);
        if (result.Succeeded)
        {
          _logger.LogInformation(
            "New user {Email} successfully registred!",
            user.Email
          );

          return Ok(result);
        }

        AddErrors(result);
      }

      return BadRequest(ModelState);
    }

    [HttpPost("changepassword")]
    [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _service.ChangePasswordAsync(model);
        if (result.Succeeded)
        {
          _logger.LogInformation(
            "The password for user {UserName} has been changed.",
            model.UserName
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
    public IActionResult DeleteAsync(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) return BadRequest();

      throw new System.NotImplementedException("DeleteAsync");
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
