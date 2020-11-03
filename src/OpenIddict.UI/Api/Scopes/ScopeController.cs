using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI
{
  [Route("api/scopes")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class ScopeController : ControllerBase
  {
    private readonly IScopeService service;

    public ScopeController(IScopeService service)
    {
      this.service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScopeViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClaimTypesAsync()
    {
      var result = await this.service.GetScopesAsync();

      return Ok(result);
    }

    [HttpGet("names")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScopeNamesAsync()
    {
      var result = await this.service.GetScopeNamesAsync();

      return Ok(result);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(ScopeViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string name)
    {
      if (name == null) return BadRequest();

      var result = await this.service.GetAsync(name);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ScopeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await this.service.CreateAsync(model);

      return Created($"api/scopes/{result}", result); // this.Json(result)
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] ScopeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await this.service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(string name)
    {
      if (name == null) return BadRequest();

      await this.service.DeleteAsync(name);

      return NoContent();
    }
  }
}