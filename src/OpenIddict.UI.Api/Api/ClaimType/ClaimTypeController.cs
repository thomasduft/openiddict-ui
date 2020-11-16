using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace tomware.OpenIddict.UI.Api
{
  [Route("api/claimtypes")]
  [Authorize(
    Policies.ADMIN_POLICY,
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
  )]
  public class ClaimTypeController : ControllerBase
  {
    private readonly IClaimTypeApiService _service;

    public ClaimTypeController(IClaimTypeApiService service)
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimTypeViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClaimTypesAsync()
    {
      var result = await _service.GetClaimTypesAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimTypeViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
      if (id != Guid.Empty) return BadRequest();

      var result = await _service.GetAsync(id);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ClaimTypeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.CreateAsync(model);

      return Created($"api/claimtypes/{result}", result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] ClaimTypeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
      if (id != Guid.Empty) return BadRequest();

      await _service.DeleteAsync(id);

      return NoContent();
    }
  }
}
