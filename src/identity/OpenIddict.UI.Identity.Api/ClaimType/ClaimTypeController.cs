using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Api
{
  [Route("claimtypes")]
  public class ClaimTypeController : ApiControllerBase
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
      if (id == Guid.Empty) return BadRequest();

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

      return Created($"claimtypes/{result}", result);
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
      if (id == Guid.Empty) return BadRequest();

      await _service.DeleteAsync(id);

      return NoContent();
    }
  }
}
