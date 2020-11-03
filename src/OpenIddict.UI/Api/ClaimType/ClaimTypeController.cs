using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI
{
  [Route("api/claimtypes")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class ClaimTypeController : ControllerBase
  {
    private readonly IClaimTypeService service;

    public ClaimTypeController(IClaimTypeService service)
    {
      this.service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimTypeViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClaimTypesAsync()
    {
      var result = await this.service.GetClaimTypesAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimTypeViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
      if (id == null) return BadRequest();

      var result = await this.service.GetAsync(id);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ClaimTypeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await this.service.CreateAsync(model);

      return Created($"api/claimtypes/{result}", result); // this.Json(result)
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] ClaimTypeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await this.service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
      if (id == null) return BadRequest();

      await this.service.DeleteAsync(id);

      return NoContent();
    }
  }
}
